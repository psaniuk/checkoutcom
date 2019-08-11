# Payment gateway API

## Assumptions 
I assumed that:
- a card number has 16 digits and starts with any number
- CVV code contains only 3 digits
- I can use GUID as an unique identifier
- a bank simulating can be achieved by mocking bank API communication. Only a success response is mocked but can be easily extended to cover all the cases

## Solution description
Solution has been built using .Net Core 3.0 using Visual Studio Code as IDE. 
It wasn't tested that any other IDE can launch and build the solution.

The solution consists of two projects:
1. _PaymentGateway_ - the main project, contains definition of endpoints and the all logic:
    - _Contracts_, the folder contains the contracts (interfaces) used in the project
    - _Controllers_, the folder for controllers logic. Process and retrieve a payment endpoints implemented in                 _PaymentGatewayController_
    - _Exceptions_, defines a module with the project exceptions
    - _Models_, contains the model used in the project. 
      Has DTO subfolder where defined data transfer objects used in client -> API or API -> dependency communications.
      The main model is described in _Payment_ class. 
      The relationship between model types described in _PaymentGatewayDbContext_
    - _Services_ folder contains implementations of repositories and HTTP clients which are defined in _Contracts_.
      _PaymentService_ class implements business logic of the main endpoints
2. _PaymentGateway.Tests_ - a unit test project for _PaymentGateway_ logic. At the moment only tests for _PaymentService_ are    implemented

## API description
## Localization
## Security
## Validation
## Idempotency
## Tests
## Request examples
## Swagger specs
