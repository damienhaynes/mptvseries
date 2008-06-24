#region GNU license
// MP-TVSeries - Plugin for Mediaportal
// http://www.team-mediaportal.com
// Copyright (C) 2006-2007
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace aclib.Performance
{
    static public class PerfWatcher
    {
        static Dictionary<string, Watch> _watches = new Dictionary<string, Watch>();
        const string anonName = "Anonymous";

        /// <summary>
        /// Returns a general purpose Watch that is useful if you don't want to create a specific Watch
        /// </summary>
        public static Watch AnonymousWatch
        {
            get { return GetNamedWatch(null); }
        }

        /// <summary>
        /// Starts the Anonymous Watch
        /// </summary>
        static public void StartWatch()
        {
            StartNamedWatch(null);
        }

        /// <summary>
        /// Starts the Watch named "name" (creates it if it doesn't exist yet)
        /// </summary>
        /// <param name="name">The Name of the Watch to Start</param>
        static public void StartNamedWatch(string name)
        {
            GetNamedWatch(name, true).Start();
        }

        /// <summary>
        /// Stops the Anonymous Watch
        /// </summary>
        static public void StopWatch()
        {
            StopNamedWatch(null);
        }

        /// <summary>
        /// Stops the Watch named "name" (does nothing if Watch doesn't exist)
        /// </summary>
        /// <param name="name">The Name of the Watch to Stop</param>
        static public void StopNamedWatch(string name)
        {
            Watch w = GetNamedWatch(name, false);
            if (w != null) w.Stop();
        }

        /// <summary>
        /// Resets the Anonymous Watch
        /// </summary>
        static public void ResetWatch()
        {
            ResetNamedWatch(null);
        }

        /// <summary>
        /// Resets the Watch named "name" (does nothing if Watch doesn't exist)
        /// </summary>
        /// <param name="name">The Name of the Watch to Reset</param>
        static public void ResetNamedWatch(string name)
        {
            Watch w = GetNamedWatch(name, false);
            if (w != null) w.Reset();
        }

        /// <summary>
        /// Releases the Watch named "name" from the internal store (does nothing if Watch doesn't exist)
        /// </summary>
        /// <param name="name">The Name of the Watch to Release</param>
        static public void ReleaseWatch(string name)
        {
            Watch w = GetNamedWatch(name, false);
            if (w == null) return;
            lock (_watches)
                _watches.Remove(w.Name);
        }

        /// <summary>
        /// Returns the Watch named "name" from the internal store (creates it if it doesn't exist)
        /// </summary>
        /// <param name="name">The Name of the Watch to Return</param>
        static public Watch GetNamedWatch(string name)
        {
            return GetNamedWatch(name, true);
        }

        /// <summary>
        /// Returns the Watch named "name" from the internal store
        /// </summary>
        /// <param name="name">The Name of the Watch to Return</param>
        /// <param name="CreateNewIfNotExists">Indicates wether a new Watch should be instantiated with the given name if it doesn't exist yet</param>
        /// <returns></returns>
        static public Watch GetNamedWatch(string name, bool CreateNewIfNotExists)
        {
            Watch watch = null;
            if (name == null) name = anonName; // for null we use a generic Anonymouse name

            if ((watch = findWatch(name)) == null && CreateNewIfNotExists)
                watch = new Watch(name); // the constructor will call AddWatch                
            return watch;
        }

        /// <summary>
        /// Adds the Watch to the internal store, replacing an existing Watch with the same name
        /// Note: very rarely will you need to call this yourself
        /// </summary>
        /// <param name="w">The Watch to add</param>
        static public void AddWatch(Watch w)
        {
            lock (_watches)
            {
                if (GetNamedWatch(w.Name, false) == null)
                    _watches.Add(w.Name, w);
                else
                { // replace
                    _watches.Remove(w.Name);
                    _watches.Add(w.Name, w);
                }
            }
        }

        /// <summary>
        /// Returns all Watches that have been instantiated
        /// </summary>
        static public IEnumerable<Watch> InstantiatedWatches
        {
            get
            {
                foreach (KeyValuePair<string, Watch> kv in _watches)
                    yield return kv.Value;
            }
        }

        static Watch findWatch(string name)
        {
            Watch w = null;
            _watches.TryGetValue(name, out w);
            return w;
        }
    }

    public sealed class Watch : Stopwatch, IComparable<Watch>
    {
        string _name = null;
        /// <summary>
        /// The Name (Identifier) of this Watch
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        int _startCounter;
        /// <summary>
        /// Returns a value indicating how often this Watch has been started since it's last Reset
        /// </summary>
        public int StartCounter
        {
            get { return _startCounter; }
        }

        /// <summary>
        /// Instantiates a new Watch with the given name
        /// </summary>
        /// <param name="name">The Name (Identifier) of this Watch</param>
        public Watch(string name)
        {
            _name = name;
            PerfWatcher.AddWatch(this);
        }

        /// <summary>
        /// Starts this Watch
        /// </summary>
        public new void Start()
        {
            _startCounter++;
            base.Start();
        }

        /// <summary>
        /// Resets this Watch' Timer as well as the StartCounter
        /// </summary>
        public new void Reset()
        {
            _startCounter = 0;
            base.Reset();
        }

        /// <summary>
        /// Returns a textual representation of this Watch, including formatted results
        /// </summary>
        public string Info
        {
            get
            {
                long msStart;
                long usStart;
                if (this.StartCounter > 0)
                {
                    msStart = this.ElapsedMilliseconds / this.StartCounter;
                    usStart = this.ElapsedMicroseconds / this.StartCounter;

                    return string.Format("Watch: Started: {1} time(s) - Elapsed: {2}ms ({3} µs) -> {4} ms ({5} µs)/start \"{0}\"",
                                     this.Name, this.StartCounter, this.ElapsedMilliseconds, this.ElapsedMicroseconds, msStart, usStart);
                }
                else return string.Format("Watch: {0} - Started: never", this.Name);
            }
        }

        /// <summary>
        /// Returns a textual representation of this Watch, including formatted results
        /// </summary>
        public override string ToString()
        {
            return this.Info;
        }

        /// <summary>
        /// Gets the total elapsed time measured by the current instance, in microseconds
        /// </summary>
        public long ElapsedMicroseconds
        {
            get { return (long)(Elapsed.Ticks / 10); }
        }

        #region IComparable<Watch> Members

        public int CompareTo(Watch other)
        {
            return this._name.CompareTo(other._name);
        }

        #endregion
    }

}
