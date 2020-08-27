namespace PremiumAccountApplications
{
    public class GameAccountApplicationEvaluator
    {
        private readonly IPremiumAccountValidator _validator;

        private const int AutoReferralMaxAge = 18;
        private const decimal HighIncomeThreshold = 100;
        private const decimal LowIncomeThreshold = 20_00;

        public GameAccountApplicationEvaluator(IPremiumAccountValidator validator)
        {
            _validator = validator;
        }


        public PremiumAccountApplicationDecision Evaluate(GameAccountApplication application)
        {
            if (application.Amount >= HighIncomeThreshold)
            {
                return PremiumAccountApplicationDecision.AutoAccepted;
            }

            if (_validator.ServiceInformation.License.LicenseKey == "EXPIRED")
            {
                return PremiumAccountApplicationDecision.ReferredToHuman;
            }

            _validator.ValidationMode = application.Age <= 18 ? ValidationMode.Detailed : ValidationMode.Quick;

            var isValidPremiumAccount =
            _validator.IsValid(application.PremiumAccountNumber);

            if (!isValidPremiumAccount)
            {
                return PremiumAccountApplicationDecision.ReferredToHuman;
            }

            if (application.Age <= AutoReferralMaxAge)
            {
                return PremiumAccountApplicationDecision.ReferredToHuman;
            }

            if (application.Amount < LowIncomeThreshold)
            {
                return PremiumAccountApplicationDecision.AutoDeclined;
            }

            return PremiumAccountApplicationDecision.ReferredToHuman;
        }

        public PremiumAccountApplicationDecision EvaluateWithOut(GameAccountApplication application)
        {
            if (application.Amount >= HighIncomeThreshold)
            {
                return PremiumAccountApplicationDecision.AutoAccepted;
            }

            if (_validator.ServiceInformation.License.LicenseKey == "EXPIRED")
            {
                return PremiumAccountApplicationDecision.ReferredToHuman;
            }



            var isValidFrequentFlyerNumber = true;
            _validator.IsValid(application.PremiumAccountNumber, out isValidFrequentFlyerNumber);

            if (!isValidFrequentFlyerNumber)
            {
                return PremiumAccountApplicationDecision.ReferredToHuman;
            }

            if (application.Age <= AutoReferralMaxAge)
            {
                return PremiumAccountApplicationDecision.ReferredToHuman;
            }

            if (application.Amount < LowIncomeThreshold)
            {
                return PremiumAccountApplicationDecision.AutoDeclined;
            }

            return PremiumAccountApplicationDecision.ReferredToHuman;
        }
    }
}
    

