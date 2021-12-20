using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace com.workflowconcepts.applications.uccx
{
    public static class Utilities
    {
        public static bool IncrementSessionIDSeqByOne(string SessionIDSeq, out string NewSessionIDSeq)
        {
            try
            {
                NewSessionIDSeq = string.Empty;

                if (string.IsNullOrEmpty(SessionIDSeq))
                {
                    Trace.TraceWarning($"{nameof(SessionIDSeq)} is null/empty");

                    return false;
                }

                string[] arSessionIDSeq = SessionIDSeq.Split(':');

                if (arSessionIDSeq != null)
                {
                    if (arSessionIDSeq.Length == 0)
                    {
                        Trace.TraceWarning($"{nameof(arSessionIDSeq)} is empty -> unexpected scenario");

                        return false;
                    }
                    else if (arSessionIDSeq.Length == 1)
                    {
                        Trace.TraceWarning($"{nameof(arSessionIDSeq)} contains only one item -> no seq detected");

                        NewSessionIDSeq = SessionIDSeq + ":1";

                        return true;
                    }
                    else if (arSessionIDSeq.Length == 2)
                    {
                        int iSeq = 0;

                        if (int.TryParse(arSessionIDSeq[1], out iSeq))
                        {
                            if (iSeq < 0)
                            {
                                iSeq = 0;
                            }

                            if (iSeq >= (int.MaxValue - 100))
                            {
                                iSeq = 0;
                            }

                            iSeq++;

                            NewSessionIDSeq = $"{arSessionIDSeq[0]}:{iSeq}";

                            return true;
                        }
                        else
                        {
                            Trace.TraceWarning($"{nameof(arSessionIDSeq)}[1]:{arSessionIDSeq[1]} is not an integer");

                            return false;
                        }
                    }
                    else
                    {
                        Trace.TraceWarning($"{nameof(arSessionIDSeq)} contains more than 2 entries -> multiple sequence markers in token");

                        return false;
                    }
                }
                else
                {
                    Trace.TraceWarning($"{nameof(arSessionIDSeq)} is null -> unexpected scenario");

                    return false;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"Exception:{ex.Message}{Environment.NewLine}Stacktrace:{ex.StackTrace}");
                NewSessionIDSeq = string.Empty;
                return false;
            }
        }
    }
}
