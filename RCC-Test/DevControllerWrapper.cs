using System;

namespace RCC
{
    public class DevControllerWrapper : IDisposable
    {
        DevController dev;
        public DevControllerWrapper(string host)
        {
            dev = new DevController(host);
        }
        // Summary:
        //     Adds the given card to the device If false is returned, check the LastErr for
        //     reason
        //
        // Parameters:
        //   dev:
        //
        //   card:
        public bool Add(string dev, string card)
        {
            return this.dev.Add(dev, card);
        }
        //
        // Summary:
        //     The Error message of the last failed call.
        public string LastErr { get { return dev.LastErr; } }
        //
        // Summary:
        //     Removes the given card from the device If false is returned, check the LastErr
        //     for reason
        //
        // Parameters:
        //   dev:
        //
        //   card:
        public bool Del(string dev, string card)
        {
            return this.dev.Del(dev, card);
        }

        public void Dispose()
        {
            dev.Dispose();
        }

        //
        // Summary:
        //     Saves the changes done to the device If false is returned, check the LastErr
        //     for reason
        //
        // Parameters:
        //   dev:
        public bool Save(string dev)
        {
            return this.dev.Save(dev);
        }

        //
        // Summary:
        //     Update the DB with offline logs from the device If false is returned, check the
        //     LastErr for reason
        //
        // Parameters:
        //   dev:
        public bool Sync(string dev)
        {
            return this.dev.Sync(dev);
        }

        //
        // Summary:
        //     List the codes registered in the device If null is returned, check the LastErr
        //     for reason
        //
        // Parameters:
        //   dev:
        public string[] List(string dev)
        {
            return this.dev.List(dev);
        }
    }
}
