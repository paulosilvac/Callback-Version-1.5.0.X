using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public class InterprocessCommunicationServer : _InterprocessCommunicationBase
    {
        public override PerformanceCountersSummary GetPerformanceCounters()
        {
            return _PerformanceCounters.GetSummary();
        }

        public override bool ResetPerformanceCounters()
        {
            return true;
        }

        public override MemoryCounters GetMemoryCounters()
        {
            return new MemoryCounters();
        }

        public override ApplicationTypes.iApplicationSettings GetApplicationSettings()
        {
            return _ApplicationSettings;
        }

        public override bool SaveApplicationSettings(ApplicationTypes.iApplicationSettings Settings)
        {
            Trace.TraceInformation("Enter");

            try
            {
                if (_SettingsChangedCallBack != null)
                {
                    _SettingsChangedCallBack(Settings);
                }

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception:" + ex.Message + Environment.NewLine + "StackTrace:" + ex.StackTrace);

                return false;
            }
        }

        public void InvokeSettingsChanged(ApplicationTypes.iApplicationSettings Settings)
        {
            if (_SettingsChangedCallBack != null)
            {
                _SettingsChangedCallBack(Settings);
            }
        }
    }
}
