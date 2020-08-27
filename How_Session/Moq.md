# Moq

[Docs](https://github.com/Moq/moq4/wiki/Quickstart)

## What is Moq

### Moq Defined

Moq is intended to be simple to use, strongly typed (no magic strings!, and therefore full compiler-verified and refactoring-friendly) and minimalistic (while still fully functional!).

Mock object gives us the ability to mimic the behavior of classes and interfaces, letting interact with those objects as if the where real implementation. We can create instances and set them up call methods and verify behavior and state.

## Why Use Moq

- Simple
- Strongly Typed
- Minimal Setup
- Fully Functional
- Less Brittle Test

## Getting Started With Moq

We use interfaces not implementations in best practice of Moq

## Install Moq

```XML
         <PackageReference Include="Moq" Version="4.13.1" />
         <PackageReference Include="FluentAssertions" Version="5.10.3" />
```

First we need the Nuget Package.

1. Right click on the Test project and select Manage NuGet packages... then goto the Browse and paste or type in Moq.

```C#
       using Moq;
```


## Configure Mock Objects

1. Lets setup our first Mock object to provide a dependency that our Game Account Evaluator requires in it's construction. As demonstrated below you can see this class needs a `IPremiumAccountValidator`

```C#

         public GameAccountApplicationEvaluator(IPremiumAccountValidator validator)
        {
            _validator = validator;
        }
```

2. We create mock object using the an interface of the Implementing class we wish to mock like so

```C#
        var mockValidator = new Mock<IPremiumAccountValidator>();
```

OR

```C#
        Mock<IPremiumAccountValidator> mockValidator = new Mock<IPremiumAccountValidator>();
```

3. Now we use the new mockValidator we created by passing it into the dependent object and we must use the Object `mockValidator.Object`

4. Lets examine the Mock namespace by right click on on Mock
in `Mock<IPremiumAccountValidator>()`

```C#
namespace Moq
{
    ...

        //
        // Summary:
        //     Exposes the mocked object instance.
        public virtual T Object { get; }

```

>Note: Examine on of the Methods and Properties like Setup, Raise,  SetupProperties.

4. Lets create our first test using a mock of the `PremiumAccountNumberValidatorService` which implements the `IPremiumAccountValidator`

```C#
       [Fact]
        public void AcceptHighIncomeApplications(){

            var mockValidator = new Mock<IPremiumAccountValidator>();

            var sut = new GameAccountApplicationEvaluator(mockValidator.Object);

            var application = new GameAccountApplication { Amount = 100_000 };

            PremiumAccountApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(PremiumAccountApplicationDecision.AutoAccepted, decision);
        };
```

## Strict And Loose

Examples:
`Mock<IInterface> mock = new Mock<IInterface>(MockBehavior.Loose);`
`var mock = new Mock<IInterface>(MockBehavior.Strict);`

- Loose Mock
  - Fewer lines of setup code
  - Setup only what is relevant
  - Default values
  - Less brittle
- Strict Mock
  - More lines of setup code
  - May have to setup irrelevant items
  - Have to setup each method call
  - More brittle

### Example of Strict:

```C#

public void AcceptHighAmountApplications(){

           Mock<IPremiumAccountValidator> mockValidator = new Mock<IPremiumAccountValidator>(MockBehavior.Strict);

            var sut = new GameAccountApplicationEvaluator(mockValidator.Object);



            var application = new GameAccountApplication { Amount = 100_000 };

            PremiumAccountApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(PremiumAccountApplicationDecision.AutoAccepted, decision);
        }

```

# Configure Mock Methods

## Setup Methods to Return Specific Values

```C#
        [Fact]
        public void ReferYoungApplications(){


            var mockValidator = new Mock<IPremiumAccountValidator>();

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            var sut = new GameAccountApplicationEvaluator(mockValidator.Object);

            var application = new GameAccountApplication { Age = 17 };

            PremiumAccountApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(PremiumAccountApplicationDecision.ReferredToHuman, decision);
        }
```

## Avoid Null Reference

1. A way to avoid Null Reference is to use the `DefaultValue.Mock` by setting the Mock DefaultValue property ` mockValidator.DefaultValue = DefaultValue.Mock;`.

2. Alternatively we can just set up Propert with the Setup Mock `Setup()` Method.

```C#
        [Fact]
        public void ReferYoungApplications(){


            var mockValidator = new Mock<IPremiumAccountValidator>();

            // Avoid Null Reference with DefaultValue
            mockValidator.DefaultValue = DefaultValue.Mock;

            //Or you can set up  property to avoid null reference execption
          //  mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            var sut = new GameAccountApplicationEvaluator(mockValidator.Object);

            var application = new GameAccountApplication { Age = 17 };

            PremiumAccountApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(PremiumAccountApplicationDecision.ReferredToHuman, decision);
        }
```

## Arguement Matching

```C#
        [Fact]
        public void DeclineLowAmountApplications()
        {
            var mockValidator = new Mock<IPremiumAccountValidator>();


            mockValidator.Setup(x => x.IsValid("x")).Returns(true);


            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new GameAccountApplicationEvaluator(mockValidator.Object);

            var application = new GameAccountApplication
            {
                Amount = (decimal)19.99,
                Age = 42,
                PremiumAccountNumber = "y"
            };

            PremiumAccountApplicationDecision decision = sut.Evaluate(application);


            decision.Should().Be(PremiumAccountApplicationDecision.AutoDeclined);




        }
```
If you test does not require a specific value you can use the `IsAny<string>()` method in place of the literal value as shown below.

```C#
        [Fact]
        public void DeclineLowAmountApplications()
        {
            var mockValidator = new Mock<IPremiumAccountValidator>();


             //mockValidator.Setup(x => x.IsValid("x")).Returns(true);

             // replace literal string with It.IsAny<string>() as the argument
             mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new GameAccountApplicationEvaluator(mockValidator.Object);

            var application = new GameAccountApplication
            {
                Amount = (decimal)19.99,
                Age = 42,
                PremiumAccountNumber = "y"
            };

            PremiumAccountApplicationDecision decision = sut.Evaluate(application);


            decision.Should().Be(PremiumAccountApplicationDecision.AutoDeclined);

        }
```
We can using predicate which returns a boolean result with a lambda function `It.Is<string>(number => number.StartsWith("y")))).Returns(true);` to see if the value begins with `y`
```C#
        [Fact]
        public void DeclineLowAmountApplications()
        {
            var mockValidator = new Mock<IPremiumAccountValidator>();


             //mockValidator.Setup(x => x.IsValid("x")).Returns(true);

             // replace literal string with It.IsAny<string>() as the argument
             //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

             //using predicate which returns a boolean result

             mockValidator.Setup(x => x.IsValid(It.Is<string>(number => number.StartsWith("y")))).Returns(true);

            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new GameAccountApplicationEvaluator(mockValidator.Object);

            var application = new GameAccountApplication
            {
                Amount = (decimal)19.99,
                Age = 42,
                PremiumAccountNumber = "y111"
            };

            PremiumAccountApplicationDecision decision = sut.Evaluate(application);


            decision.Should().Be(PremiumAccountApplicationDecision.AutoDeclined);




        }
```


We can also check if a value is within a specific range by passing in the `It.IsInRange<string>("a", "z", Moq.Range.Inclusive))`

```C#
        [Fact]
        public void DeclineLowAmountApplications()
        {
            var mockValidator = new Mock<IPremiumAccountValidator>();

            //Is any method
            // mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            // It Is String with lambda
            // mockValidator.Setup(x => x.IsValid(It.Is<string>(number => number.StartsWith("y")))).Returns(true);

            // It Is In Range Inclusive includes a and z Exclusive b - y
            mockValidator.Setup(x => x.IsValid(It.IsInRange<string>("a", "z", Moq.Range.Inclusive))).Returns(true);



            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new GameAccountApplicationEvaluator(mockValidator.Object);

            var application = new GameAccountApplication
            {
                Amount = (decimal)19.99,
                Age = 42,
                PremiumAccountNumber = "y"
            };

            PremiumAccountApplicationDecision decision = sut.Evaluate(application);


            decision.Should().Be(PremiumAccountApplicationDecision.AutoDeclined);

        }
```
We can also check to see if value is in within a set by using `It.IsIn<string>("a", "z", "y")`

```C#
        [Fact]
        public void DeclineLowAmountApplications()
        {
            var mockValidator = new Mock<IPremiumAccountValidator>();

            //Is any method
            // mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            // It Is String with lambda
            // mockValidator.Setup(x => x.IsValid(It.Is<string>(number => number.StartsWith("y")))).Returns(true);
            // It Is In Range Inclusive includes a and z Exclusive b - y
            //mockValidator.Setup(x => x.IsValid(It.IsInRange<string>("a", "z", Moq.Range.Inclusive))).Returns(true);


            //Is In set of values passed in
             mockValidator.Setup(x => x.IsValid(It.IsIn<string>("a", "z", "y"))).Returns(true);




            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new GameAccountApplicationEvaluator(mockValidator.Object);

            var application = new GameAccountApplication
            {
                Amount = (decimal)19.99,
                Age = 42,
                PremiumAccountNumber = "y"
            };

            PremiumAccountApplicationDecision decision = sut.Evaluate(application);


            decision.Should().Be(PremiumAccountApplicationDecision.AutoDeclined);

        }
```

```C#
        [Fact]
        public void DeclineLowAmountApplications()
        {
            var mockValidator = new Mock<IPremiumAccountValidator>();

            //Is any method
            // mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            // It Is String with lambda
            // mockValidator.Setup(x => x.IsValid(It.Is<string>(number => number.StartsWith("y")))).Returns(true);
            // It Is In Range Inclusive includes a and z Exclusive b - y
            //mockValidator.Setup(x => x.IsValid(It.IsInRange<string>("a", "z", Moq.Range.Inclusive))).Returns(true);
            //Is In set of values passed in
            //mockValidator.Setup(x => x.IsValid(It.IsIn<string>("a", "z", "y"))).Returns(true);

            mockValidator.Setup(x => x.IsValid(It.IsRegex("[a-z]"))).Returns(true);


            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new GameAccountApplicationEvaluator(mockValidator.Object);

            var application = new GameAccountApplication
            {
                Amount = (decimal)19.99,
                Age = 42,
                PremiumAccountNumber = "y"
            };

            PremiumAccountApplicationDecision decision = sut.Evaluate(application);


            decision.Should().Be(PremiumAccountApplicationDecision.AutoDeclined);

        }
```

```C#
        [Fact]
        public void ReferInvalidPremiumAccountApplicaitons()
        {
            var mockValidator = new Mock<IPremiumAccountValidator>();

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            var sut = new GameAccountApplicationEvaluator(mockValidator.Object);
            var application = new GameAccountApplication();
            PremiumAccountApplicationDecision decision = sut.Evaluate(application);

            decision.Should().Be(PremiumAccountApplicationDecision.ReferredToHuman);

        }
```

```C#
        [Fact]
        public void DeclineLowAmountApplicationOutDemo()
        {
            var mockValidator = new Mock<IPremiumAccountValidator>();

            var isValid = true;
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid));
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");


            var sut = new GameAccountApplicationEvaluator(mockValidator.Object);

            var application = new GameAccountApplication
            {
                Amount = (decimal)19.99,
                Age = 42
            };

            PremiumAccountApplicationDecision decision = sut.EvaluateWithOut(application);
            decision.Should().Be(PremiumAccountApplicationDecision.AutoDeclined);
        }
```

```C#
        [Fact]
        public void ReferWhenLicenseKeyExpired()
        {

            var mockValidator = new Mock<IPremiumAccountValidator>();

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("EXPIRED");

            var sut = new GameAccountApplicationEvaluator(mockValidator.Object);
            var application = new GameAccountApplication() { Age = 42 };
            PremiumAccountApplicationDecision decision = sut.Evaluate(application);

            decision.Should().Be(PremiumAccountApplicationDecision.ReferredToHuman);


        }
```

## Can't Member or Remember?

Mock objects do not rememeber changes made to them by default so you need to use the SetupProperty(x => x.PropertyToBeRemembered) method to ensure changes made at test run time are remembered or SetupAllProperties() method

```C#
        [Fact]
        public void UseDetailedLookupForOlderApplications()
        {
            var mockValidator = new Mock<IPremiumAccountValidator>();

            //ensure you place this SetupProperties before your setup code to avoid overriding you setup with default values
            mockValidator.SetupProperty(x => x.ValidationMode);
            mockValidator.SetupAllProperties();

            //Setup code
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");


            var sut = new GameAccountApplicationEvaluator(mockValidator.Object);
            var application = new GameAccountApplication { Age = 30 };
            sut.Evaluate(application);

            mockValidator.Object.ValidationMode.Should().Be(ValidationMode.Detailed);

        }

```

```C#
        [Fact]
        public void NotValidPremiumAccountNumberValidator()
        {
            var mockValidator = new Mock<IPremiumAccountValidator>();

            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var sut = new GameAccountApplicationEvaluator(mockValidator.Object);
            var application = new GameAccountApplication() { Amount = 100 };
            sut.Evaluate(application);
            //Times Once Never Exactly for example
            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Never);
        }
    }
```

```C#
         [Fact]
        public void SetDetailedLookupForYoungerApplications()
        {
            var mockValidator = new Mock<IPremiumAccountValidator>();

            //ensure you place this SetupProperties before your setup code to avoid overriding you setup with default values
            mockValidator.SetupProperty(x => x.ValidationMode);
            mockValidator.SetupAllProperties();

            //Setup code
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");


            var sut = new GameAccountApplicationEvaluator(mockValidator.Object);
            var application = new GameAccountApplication { Age = 17 };
            sut.Evaluate(application);

            mockValidator.VerifySet(x => x.ValidationMode = ValidationMode.Detailed);
            mockValidator.VerifySet(x => x.ValidationMode = It.IsAny<ValidationMode>(), Times.Once);

            //mockValidator.VerifyNoOtherCalls();
        }
```
