using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ColonistReport
{
    public class ColonistReportsMod : IMod
    {
        GameStateGame Game { get; set; } 
        GuiReportsMenuItem ReportsMenuItem { get; set; } 
        GuiReportsMenu ReportsMenu { get; set; }

        public void Init()
        {
            RegisterStrings();

            WorkloadManager.Init();

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

        public void Update()
        {
            Game = GameManager.getInstance().getGameState() as GameStateGame;

            TryInitializeReportsMenu();

            WorkloadManager.Update();
            TryUpdateMenu();
        }

        private void TryUpdateMenu()
        {
            if (Game.mGameGui.getWindow() is GuiReportsMenu menu)
            {
                menu.updateUi();
            }
        }

        private void TryInitializeReportsMenu()
        {
            if (Game.mMenuSystem != null && Game.mMenuSystem.mMenuBaseManagement is GuiMenu menuBaseManagement)
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
