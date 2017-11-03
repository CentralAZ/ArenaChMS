using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arena.Core;

namespace Arena.Custom.Cccev.CheckIn.Entity
{
    /// <summary>
    /// Null Object class that inherits Occurrence.
    /// </summary>
    public class EmptyOccurrence : Occurrence
    {
        public EmptyOccurrence()
        {
            base.Location = "Unavailable";
        }

        public EmptyOccurrence(DateTime startTime) : this()
        {
            base.StartTime = startTime;
        }

        public override int Save(string userId)
        {
            return -1;
        }

        public override int Save(string userId, bool loadOccurrenceAttendance)
        {
            return -1;
        }
    }
}
