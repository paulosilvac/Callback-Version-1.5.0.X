using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public class DataCollectionServiceMonitor
    {

        public event EventHandler Started;
        public event EventHandler Stopped;
        public event EventHandler<MaximumNumberOfCollisionsReachedEventsArgs> MaximumNumberOfCollisionsReached;
        public event EventHandler<MaximumNumberOfErrorsReachedEventsArgs> MaximumNumberOfErrorsReached;

        const int TICKINTERVAL = 5000;
        const int MAXIMUM_NUMBER_OF_HASH_COLLISIONS = 5;
        const int MAXIMUM_NUMBER_OF_ERRORS = 5;

        System.Threading.Timer _tmrTick = null;

        private object objLock = null;

        SettingsManager _settingsManager = null;

        private String _PreviousHash = String.Empty;

        private int _NumberOfHashCollisions = 0;
        private int _NumberOfErrors = 0;

        private StringBuilder sbErrorHistory = null;

        public SettingsManager SettingsManager
        {
            get { return _settingsManager; }
            set { _settingsManager = value; }
        }

        public DataCollectionServiceMonitor()
        {
            _settingsManager = null;

            _PreviousHash = String.Empty;

            _NumberOfHashCollisions = 0;
            
            _NumberOfErrors = 0;

            sbErrorHistory = new StringBuilder();

            objLock = new object();

            _tmrTick = new System.Threading.Timer(_tmrTick_Tick);
        }

        public DataCollectionServiceMonitor(SettingsManager SettingsManager)
        {
            _settingsManager = SettingsManager;

            _PreviousHash = String.Empty;

            _NumberOfHashCollisions = 0;

            _NumberOfErrors = 0;

            sbErrorHistory = new StringBuilder();

            objLock = new object();

            _tmrTick = new System.Threading.Timer(_tmrTick_Tick);
        }

        public bool Start()
        {
            Trace.TraceInformation("Enter.");

            _PreviousHash = String.Empty;

            _NumberOfHashCollisions = 0;

            _NumberOfErrors = 0;

            sbErrorHistory.Clear();

           _tmrTick.Change(TICKINTERVAL, TICKINTERVAL);

            if (Started != null)
            {
                Started(this, new EventArgs());
            }

            return true;
        }

        public bool Stop()
        {
            Trace.TraceInformation("Enter.");

            _tmrTick.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            if (Stopped != null)
            {
                Stopped(this, new EventArgs());
            }

            return true;
        }

        void _tmrTick_Tick(object State)
        {
            Trace.TraceInformation("Enter.");

            lock (objLock)
            {
                _tmrTick.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                if (_settingsManager == null)
                {
                    Trace.TraceWarning("_settingsManager is null.");
                    _tmrTick.Change(TICKINTERVAL, TICKINTERVAL);
                    return;
                }

                if (_settingsManager.ApplicationSettings == null)
                {
                    Trace.TraceWarning("_settingsManager.ApplicationSettings is null.");
                    _tmrTick.Change(TICKINTERVAL, TICKINTERVAL);
                    return;
                }

                DataCollectionServiceInformation info = new DataCollectionServiceInformation(_settingsManager.ApplicationSettings);

                if (!info.GetInformation())
                {
                    Trace.TraceWarning("info.GetInformation() returned false.");

                    sbErrorHistory.Append("info.GetInformation() returned false.");
                    sbErrorHistory.Append(Environment.NewLine);

                    _NumberOfErrors++;

                    if (_NumberOfErrors >= MAXIMUM_NUMBER_OF_ERRORS)
                    {
                        Trace.TraceWarning("Maximum number of errors was reached. Error history: " + sbErrorHistory.ToString());

                        if (MaximumNumberOfErrorsReached != null)
                        {
                            MaximumNumberOfErrorsReached(this, new MaximumNumberOfErrorsReachedEventsArgs("Maximum number of errors was reached: " + MAXIMUM_NUMBER_OF_ERRORS + Environment.NewLine + Environment.NewLine + sbErrorHistory.ToString()));
                        }

                        _NumberOfErrors = 0;

                        sbErrorHistory.Clear();

                    }//if (_NumberOfErrors >= MAXIMUM_NUMBER_OF_ERRORS)

                    _tmrTick.Change(TICKINTERVAL, TICKINTERVAL);

                    return;
                }

                if (!info.AssertNoError())
                {
                    Trace.TraceWarning("info.AssertNoError() returned false.");

                    sbErrorHistory.Append("info.AssertNoError() returned false.");
                    sbErrorHistory.Append(Environment.NewLine);

                    _NumberOfErrors++;

                    if (_NumberOfErrors >= MAXIMUM_NUMBER_OF_ERRORS)
                    {
                        Trace.TraceWarning("Maximum number of errors was reached. Error history: " + sbErrorHistory.ToString());

                        if (MaximumNumberOfErrorsReached != null)
                        {
                            MaximumNumberOfErrorsReached(this, new MaximumNumberOfErrorsReachedEventsArgs("Maximum number of errors was reached: " + MAXIMUM_NUMBER_OF_ERRORS + Environment.NewLine + Environment.NewLine + sbErrorHistory.ToString()));
                        }

                        _NumberOfErrors = 0;

                        sbErrorHistory.Clear();

                    }//if (_NumberOfErrors >= MAXIMUM_NUMBER_OF_ERRORS)

                    _tmrTick.Change(TICKINTERVAL, TICKINTERVAL);

                    return;
                }

                if (!info.AssertNotEmpty())
                {
                    Trace.TraceWarning("info.AssertNotEmpty() returned false.");

                    sbErrorHistory.Append("info.AssertNotEmpty() returned false.");
                    sbErrorHistory.Append(Environment.NewLine);

                    _NumberOfErrors++;

                    if (_NumberOfErrors >= MAXIMUM_NUMBER_OF_ERRORS)
                    {
                        Trace.TraceWarning("Maximum number of errors was reached. Error history: " + sbErrorHistory.ToString());

                        if (MaximumNumberOfErrorsReached != null)
                        {
                            MaximumNumberOfErrorsReached(this, new MaximumNumberOfErrorsReachedEventsArgs("Maximum number of errors was reached: " + MAXIMUM_NUMBER_OF_ERRORS + Environment.NewLine + Environment.NewLine + sbErrorHistory.ToString()));
                        }

                        _NumberOfErrors = 0;

                        sbErrorHistory.Clear();

                    }//if (_NumberOfErrors >= MAXIMUM_NUMBER_OF_ERRORS)

                    _tmrTick.Change(TICKINTERVAL, TICKINTERVAL);

                    return;
                }

                String _Hash = info.GetHash();

                info = null;

                if (_Hash == String.Empty)
                {
                    Trace.TraceWarning("_Hash is empty.");

                    sbErrorHistory.Append("_Hash is empty.");
                    sbErrorHistory.Append(Environment.NewLine);

                    _NumberOfErrors++;

                    if (_NumberOfErrors >= MAXIMUM_NUMBER_OF_ERRORS)
                    {
                        Trace.TraceWarning("Maximum number of errors was reached. Error history: " + sbErrorHistory.ToString());

                        if (MaximumNumberOfErrorsReached != null)
                        {
                            MaximumNumberOfErrorsReached(this, new MaximumNumberOfErrorsReachedEventsArgs("Maximum number of errors was reached: " + MAXIMUM_NUMBER_OF_ERRORS + Environment.NewLine + Environment.NewLine + sbErrorHistory.ToString()));
                        }

                        _NumberOfErrors = 0;

                        sbErrorHistory.Clear();

                    }//if (_NumberOfErrors >= MAXIMUM_NUMBER_OF_ERRORS)

                    _tmrTick.Change(TICKINTERVAL, TICKINTERVAL);

                    return;
                }

                _NumberOfErrors = 0;
                sbErrorHistory.Clear();

                if (_PreviousHash == String.Empty)
                {
                    _NumberOfHashCollisions = 0;
                    _PreviousHash = _Hash;
                    _tmrTick.Change(TICKINTERVAL, TICKINTERVAL);
                    return;
                }

                if (_PreviousHash != _Hash)
                {
                    _NumberOfHashCollisions = 0;
                    _PreviousHash = _Hash;
                    _tmrTick.Change(TICKINTERVAL, TICKINTERVAL);
                    return;
                }

                if (_PreviousHash == _Hash)
                {
                    _NumberOfHashCollisions++;

                    if (_NumberOfHashCollisions >= MAXIMUM_NUMBER_OF_HASH_COLLISIONS)
                    {
                        Trace.TraceWarning("Maximum number of hash collisions was reached.");

                        if (MaximumNumberOfCollisionsReached != null)
                        {
                            MaximumNumberOfCollisionsReached(this, new MaximumNumberOfCollisionsReachedEventsArgs("Maximum number of hash collisions was reached: " + MAXIMUM_NUMBER_OF_HASH_COLLISIONS));
                        }

                        _NumberOfHashCollisions = 0;
                    }
                }

                _tmrTick.Change(TICKINTERVAL, TICKINTERVAL);

            }//lock (objLock)

            Trace.TraceInformation("Exit.");
        }
    }
}
