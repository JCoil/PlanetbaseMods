using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ColonistReport
{
    public class ColonistReportsMod : ModWrapper.ModBase
    {
        GuiReportsMenuItem ReportsMenuItem { get; set; } 
        GuiReportsMenu ReportsMenu { get; set; }

        public override void Init()
        {
            RegisterStrings();

            WorkloadManager.mInstance = new WorkloadManager();

            ReportsMenuItem = new GuiReportsMenuItem(new GuiDefinitions.Callback(OnReportsMenuOpen));
            ReportsMenu = new GuiReportsMenu();

            Debug.Log("[MOD] Colonist Reports activated");
        }

        private void RegisterStrings()
        {
            StringList.mStrings.Add("reports", "Base Reports");
            StringList.mStrings.Add("reports_workload", "Colonist Workload");

            StringList.mStrings.Add("reports_worker_workload", "Worker Workload");
        }

        public override void Update(float timeStep)
        {
            WorkloadManager.getInstance().Update(timeStep);

            if (Game.mGameGui.getWindow() is GuiReportsMenu menu)
            {
                menu.updateUi();
            }
        }

        public override void OnGameStart()
        {
            if (MenuSystem.mMenuBaseManagement is GuiMenu menuBaseManagement)
            {
                if (!menuBaseManagement.mItems.Contains(ReportsMenuItem))
                {
                    var insertIndex = menuBaseManagement.mBackItem == null ?
                        menuBaseManagement.getItemCount() - 1 :
                        menuBaseManagement.mItems.IndexOf(menuBaseManagement.mBackItem);

                    menuBaseManagement.mItems.Insert(insertIndex, ReportsMenuItem);
                }
            }
        }

        private void OnReportsMenuOpen(object parameter)
        {
            if (Game.mGameGui.getWindow() is GuiReportsMenu)
            {
                Game.mGameGui.setWindow(null);
            }
            else
            {
                Game.mGameGui.setWindow(ReportsMenu);
            }
        }
    }
}
