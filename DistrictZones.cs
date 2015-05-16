using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using UnityEngine;
using ColossalFramework;
using System.Reflection;
using ColossalFramework.Threading;
using System.Collections;

namespace DistrictZones {

  public class DistrictZonesMod : IUserMod {
    public string Description {
      get { return "Display Zones with Districts Overlay"; }
    }

    public string Name {
      get { return "Show Zones with Districts"; }
    }
  }

  public class DZThreadingExtension : ThreadingExtensionBase {

    private IThreading threading;
    private readonly DistrictZonesUI streetDirectionViewerUI;

    private bool uiCreated = false;

    public DZThreadingExtension() {
      streetDirectionViewerUI = new DistrictZonesUI();
    }

    public override void OnCreated(IThreading threading) {
      this.threading = threading;
    }

    public override void OnUpdate(float realTimeDelta, float simulationTimeDelta) {
      if (!uiCreated) {
        try {
          streetDirectionViewerUI.appMode = threading.managers.loading.currentMode;
          streetDirectionViewerUI.CreateUI();
        } catch (Exception e) {
          CitiesConsole.Error(e);
        }
        uiCreated = true;
      }
    }
  }
}
