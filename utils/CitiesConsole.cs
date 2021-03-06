﻿using ColossalFramework.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DistrictZones {
  class CitiesConsole {

    private const string prefix = "DistrictZones: ";

    public static void Log(Object s) {
      DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, prefix + ObjectToString(s));
    }

    public static void Error(Object s) {
      DebugOutputPanel.AddMessage(PluginManager.MessageType.Error, prefix + ObjectToString(s));
    }

    public static void Warning(Object s) {
      DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, prefix + ObjectToString(s));
    }

    private static string ObjectToString(Object s) {
      return s == null ? "(null)" : s.ToString();
    }
  }
}
