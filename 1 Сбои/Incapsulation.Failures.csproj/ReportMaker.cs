using System;
using System.Collections.Generic;

namespace Incapsulation.Failures
{
    internal class ReportMaker
    {
        enum FailureType : int
        {
            unexpectedShutdown,
            shortNonResponding,
            hardwareFailures,
            connectionProblems
        }

        private class Device
        {
            internal int[] deviceId;
            internal object[][] times;
            internal List<Dictionary<string, object>> devices;
            internal Device(
            int[] deviceId,
            object[][] times,
            List<Dictionary<string, object>> devices)
            {
                this.deviceId = deviceId;
                this.times = times;
                this.devices = devices;
            }
        }

        private class Failure
        {
            internal int[] failureTypes;
            internal Failure(
                int[] failureTypes)
            {
                this.failureTypes = failureTypes;
            }
        }

        private static int IsFailureSerious(int failureType)
        {
            if (Enum.IsDefined(typeof(FailureType), failureType) && failureType % 2 == 0) return 1;
            return 0;
        }

        private static int IsEarlier(object[] v, int day, int month, int year)
        {
            var vYear = (int)v[2];
            var vMonth = (int)v[1];
            var vDay = (int)v[0];
            if (vYear < year) return 1;
            if (vYear > year) return 0;
            if (vMonth < month) return 1;
            if (vMonth > month) return 0;
            if (vDay < day) return 1;
            return 0;
        }

        internal static List<string> FindDevicesFailedBeforeDateObsolete(
            int day,
            int month,
            int year,
            int[] failureTypes, 
            int[] deviceId, 
            object[][] times,
            List<Dictionary<string, object>> devices)
        {
            DateTime date = new DateTime(year, month, day);
            Failure failureTypesForCheck = new Failure(failureTypes);
            Device devicesForCheck = new Device(deviceId, times, devices);
            return FindDevicesFailedBeforeDate(date, failureTypesForCheck, devicesForCheck);
        }

        private static List<string> FindDevicesFailedBeforeDate(
            DateTime date, Failure failure, Device devices)
        {
            var day = date.Day;
            var month = date.Month;
            var year = date.Year;
            int[] failureTypes = failure.failureTypes;
            int[] deviceId = devices.deviceId;
            object[][] times = devices.times;
            List<Dictionary<string, object>> devicesInfo = devices.devices;
            var problematicDevices = new HashSet<int>();
            for (var i = 0; i < failureTypes.Length; i++)
                if (IsFailureSerious(failureTypes[i]) == 1 && IsEarlier(times[i], day, month, year) == 1)
                    problematicDevices.Add(deviceId[i]);

            var result = new List<string>();
            foreach (var device in devicesInfo)
                if (problematicDevices.Contains((int)device["DeviceId"]))
                    result.Add(device["Name"] as string);

            return result;
        }
    }
}