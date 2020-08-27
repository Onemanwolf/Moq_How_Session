using System;

namespace PremiumAccountApplications
{
    
    public class PremiumAccountNumberValidatorService : IPremiumAccountValidator
    {
       //public string LicenseKey => throw new NotImplementedException();

        public ValidationMode ValidationMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IServiceInformation ServiceInformation => throw new NotImplementedException();

        public bool IsValid(string frequentFlyerNumber)
        {
            throw new NotImplementedException("Simulate this real dependency being hard to use");
        }

        public void IsValid(string frequentFlyerNumber, out bool isValid)
        {
            throw new NotImplementedException("Simulate this real dependency being hard to use");
        }
    }
}
