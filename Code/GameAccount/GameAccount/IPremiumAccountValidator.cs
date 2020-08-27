using System;

namespace PremiumAccountApplications
{
    public interface ILicenseData
    {
        string LicenseKey { get; }
    }

    public interface IServiceInformation {
    
        ILicenseData License { get; }
    }



    public interface IPremiumAccountValidator
    {
        bool IsValid(string premiumAccountNumber);
        void IsValid(string premiumAccountNumber, out bool isValid);

        IServiceInformation ServiceInformation { get; }
        ValidationMode ValidationMode { get; set; }
    }
}