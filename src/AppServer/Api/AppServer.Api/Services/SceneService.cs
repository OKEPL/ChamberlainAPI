using System;
using System.Collections.Generic;
using System.Linq;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Scene;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.AppServer.Api.Helpers;
using Chamberlain.Common.Content.StructureMapContent;
using Chamberlain.Database.Persistency.Model;
using Chamberlain.Database.Persistency.Model.Enums;
using Chamberlain.Database.Persistency.Model.Extensions;
using Microsoft.EntityFrameworkCore;
using PredefinedRulesManager.SceneRules;
using Serilog;

namespace Chamberlain.AppServer.Api.Services
{
    public class SceneService : ISceneService
    {
        public List<SceneModel> GetScenes(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Scenes).First(x => x.Username.Equals(userName));
                return customer.Scenes.Where(s => s.CustomerId == customer.Id).AsEnumerable()
                    .Select(s => s.ToSceneModel())
                    .ToList();
            }
        }

        public SceneModel GetScene(string userName, long sceneId)
        {
            using (var context = new Entities())
            {
                var scene = context.Scenes.FirstOrDefault(s => s.Id == sceneId);
                if (scene == null)
                    throw new ArgumentNullException();
                return scene.ToSceneModel();
            }
        }

        public SceneModel AddScene(string userName, SceneModel sceneModel)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(x => x.Username.Equals(userName));
                
                var scene = context.Scenes.Add(new Scene
                {
                    Name = sceneModel.Name,
                    CustomerId = customer.Id
                }).Entity;
                context.SaveChanges();

                return new SceneModel
                {
                    Name = sceneModel.Name,
                    SceneId = scene.Id
                };
            }
        }

        public List<SceneThingNamed> GetSceneThings(string userName, long sceneId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Scenes).ThenInclude(x => x.ScenesThingsBindings).First(x => x.Username.Equals(userName));

                
                var scene = customer.Scenes.First(s => s.Id == sceneId);
                
                var devices = scene.ScenesThingsBindings.Where(x => x.Thing.State == ThingStates.Active).Select(s => new SceneThingNamed
                {
                    ThingId = s.Thing.Id,
                    ThingName = s.Thing.GivenName
                });

                return devices.ToList();
            }
        }

        public SceneThingModel AddThingToScene(string userName, SceneThingModel sceneThingModel)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(x => x.Username.Equals(userName));
                
                var thing = customer.Things.FirstOrDefault(t => t.Id == sceneThingModel.ThingId);
                var scene = customer.Scenes.FirstOrDefault(s => s.Id == sceneThingModel.SceneId);
                if (scene == null || thing == null)
                    throw new ArgumentNullException();

                if (context.ScenesThingsBindings.Any(x => x.SceneId == scene.Id && x.ThingId == thing.Id))
                    throw new ArgumentException("Thing already added to this scene");

                var sceneThingBindings = new ScenesThingsBinding
                {
                    SceneId = scene.Id,
                    ThingId = thing.Id
                };

                context.ScenesThingsBindings.Add(sceneThingBindings);
                context.SaveChanges();
            }

            ObjectFactory.Container.GetInstance<IPredefinedSceneRulesManagerPlugin>().InitializePredefinedRulesForAThingInScene(sceneThingModel.ThingId, sceneThingModel.SceneId);

            return sceneThingModel;
        }

        public void DeleteScene(string userName, long sceneId)
        {
            Scene scene;
            List<ScenesThingsBinding> bindings;

            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Scenes).First(x => x.Username.Equals(userName));
                scene = customer.Scenes.FirstOrDefault(s => s.Id == sceneId);

                if (scene == null)
                    return;

                bindings = scene.ScenesThingsBindings.ToList();
            }

            foreach (var binding in bindings)
                DeleteThingFromScene(userName, binding.SceneId, binding.ThingId);

            using (var context = new Entities())
                context.Scenes.Single(x => x.Id == scene.Id).RemoveSafely(context);
        }

        public void DeleteThingFromScene(string userName, long sceneId, long thingId)
        {
            try
            {
                using (var context = new Entities())
                {
                    var sceneThingBinding = context.ScenesThingsBindings
                        .FirstOrDefault(s =>
                            s.SceneId == sceneId && s.ThingId == thingId && s.Scene.Customer.Username == userName);

                    if (sceneThingBinding == null)
                        return;

                    context.ScenesThingsBindings.Remove(sceneThingBinding);
                    context.SaveChanges();
                }

                ObjectFactory.Container.GetInstance<IPredefinedSceneRulesManagerPlugin>()
                    .UpdateRulesOnThingRemovedFromScene(thingId, sceneId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error removing thing {thingId} from scene {sceneId}");
            }
        }

        public SceneModel UpdateScene(string userName, SceneModel sceneModel)
        {
            List<ScenesRulesBinding> bindings;
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Scenes).First(x => x.Username.Equals(userName));
                
                var scene = customer.Scenes.FirstOrDefault(s => s.Id == sceneModel.SceneId);

                if (scene == null)
                    throw new ArgumentNullException();

                bindings = scene.ScenesRulesBindings.ToList();

                scene.Name = sceneModel.Name;
                context.SaveChanges();

                foreach (var binding in bindings)
                {
                    UpdateNameRuleOnScene(userName, binding.SceneId, binding.RuleId);
                }
                return sceneModel;
            }
        }

        private void UpdateNameRuleOnScene(string userName, long sceneId, long ruleId)
        {
           using (var context = new Entities())
            {
                var sceneRuleBinding = context.ScenesRulesBindings
                    .FirstOrDefault(s =>
                    s.SceneId == sceneId && s.RuleId == ruleId && s.Scene.Customer.Username == userName);

                var predefinedRuleName = sceneRuleBinding.Rule.PredefinedRule.Name;

                sceneRuleBinding.Rule.Name = $"{predefinedRuleName} in the {sceneRuleBinding.Scene.Name}";

                context.SaveChanges();
            }
        }
    }
}
