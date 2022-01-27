using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StorageGuru
{
    public abstract class RedirectModule : Module
    {
        public static Module FindStorage(Character character)
        {
            float num = float.MaxValue;
            Module result = null;
            Vector3 position = character.getPosition();

            List<Module> list = mModuleCategories[2];

            if (character.getLoadedResource() is Resource resource)
            {
                list = StorageGuruMod.StorageController.ListValidModules(resource.getResourceType());
            }
            
            if (list != null)
            {
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    Module module = list[i];
                    if (module.isOperational() && module.isSurvivable(character) && module.getEmptyStorageSlotCount() > module.getPotentialUserCount(character))
                    {
                        try
                        {
                            float sqrMagnitude = (module.getPosition() - position).sqrMagnitude;
                            if (sqrMagnitude < num)
                            {
                                result = module;
                                num = sqrMagnitude;
                            }
                        }
                        catch(NullReferenceException)
                        {
                            // Looks like a module's been deleted
                            StorageGuruMod.StorageController.ConsolidateManifest();
                        }
                    }
                }
            }

            return result;
        }
    }
}
