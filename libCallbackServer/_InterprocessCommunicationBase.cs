using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.workflowconcepts.applications.uccx
{
    public abstract class _InterprocessCommunicationBase : MarshalByRefObject
    {
        protected static PerformanceCounters _PerformanceCounters = null;
        protected static MemoryCounters _MemoryCounters = null;
        protected static ApplicationTypes.iApplicationSettings _ApplicationSettings = null;
        protected static com.vanticore.utilities.debug.DebugManager _DebugManager = null;
        protected static ApplicationTypes.SettingsCallback _SettingsChangedCallBack = null;

        public abstract bool ResetPerformanceCounters();
        public abstract PerformanceCountersSummary GetPerformanceCounters();
        public abstract MemoryCounters GetMemoryCounters();
        public abstract ApplicationTypes.iApplicationSettings GetApplicationSettings();
        public abstract bool SaveApplicationSettings(ApplicationTypes.iApplicationSettings Settings);

        public static void SetSettingsChangedCallBack(ApplicationTypes.SettingsCallback Reference)
        {
            _SettingsChangedCallBack = Reference;
        }

        public static void SetPerformanceCountersReference(PerformanceCounters Reference)
        {
            _PerformanceCounters = Reference;
        }

        public static void SetMemoryCountersReference(MemoryCounters Reference)
        {
            _MemoryCounters = Reference;
        }

        public static void SetDebugManagerReference(com.vanticore.utilities.debug.DebugManager Reference)
        {
            _DebugManager = Reference;
        }

        public static void SetApplicationSettingsReference(ApplicationTypes.iApplicationSettings Reference)
        {
            _ApplicationSettings = Reference;
        }
    }
}
