# Payment gateway API

## Assumptions 
I assumed that:
- a card number has 16 digits and starts with any number
- CVV code contains only 3 digits
- I can use GUID as an unique identifier
- a bank simulating can be achieved by mocking bank API communication. Only a success response is mocked but can be easily extended to cover all the cases
- Swagger specs schould be used for the API description

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
Payment gateway API exposes the following endpoints:
   - _GET /health_, it's used for the health check
   - _GET /payments/{id}_, the endpoint retrieves a payment details by id. ID should be provided in a query path and be a GUID 
   - _POST /payments_, the endpoint processes a payment.
     Payload: 
```
{
	"cardNumber": "1234123412341234",
	"amount": 2010,
	"cvv": "123",
	"currency": "EUR",
	"expiryAt": "12/23"
}
 ```
_Amount_ is Int32 number, which represents a paymnent amount without a decimal separator. It's converted to a decimal number by dividing by 100. For example, when 21 is given as amount, the API will convert it to 21.00. When 234 given as a payment amount, the API will convert it to 23.40. 

Swagger specs ideally should be used to describe the API, but not introduced in the scope of the given task
  
## Localization
At the moment the API supports _en-US_ and _de-DE_ cultures but can be extended.

## Security
The solution relies only on the security level provided by _HTTPS_ protocl, which is not enought for a real payment API.
More advanced security with Authorization is required.

## Idempotency
_POST /payments_ endpoints required _Idempotency-Key_ header which is GUID. IdempotencyKey object is a pair of unique ID of a payment operation and a payment ID. The endpoint validates provided by a client idempotency key. An error is returned if an idempotency key is not valid. 

## Test API
I prefer to test the API using `curl` or `httpie` with `jq` to format the response. If you don't use `jq` then `| jq "."` can be omitted.
_POST /payments_
```
curl -d '{ "cardNumber": "1234123412341234", "amount": 2010, "cvv": "123", "currency": "EUR", "expiryAt": "12/23" }' \
-H "Idempotency-Key:87856edf-25a6-4d3f-893a-1cfdc077f218" \
-H "Content-Type: application/json" \
-X POST https://localhost:5001/payments --insecure | jq "."
```
_GET /payments/{id}_
```
curl https://localhost:5001/payments/fdda6719-5314-4418-b126-90ee039c89dc --insecure | jq "."
```
