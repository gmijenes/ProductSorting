# ProductSorting API

The ProductSorting API is a RESTful application built with .NET 6 that allows you to sort stock based on the number of items in inventory and sales.

## Table of Contents
- [Dependencies and Development](#dependencies-and-development)
- [Testing](#Testing)
- [Running as container](#running-as-container)
- [Making requests to the container](#making-requests-to-the-container)

## Dependencies and Development
### Main Technologies:
* .NET 6 SDK
* Docker
* Postman

### Development steps:
1. **Fork the repository** from GitHub root https://github.com/gmijenes/ProductSorting.git

2. **Install dependencies:** Make sure you have the .NET 6 SDK installed. If not, download it [here](https://dotnet.microsoft.com/es-es/download/dotnet/6.0). Then restore the NuGet packages in the project directory using 
` dotnet restore `. 

## Testing 
When running the application in development mode, you have access to the Swagger UI for testing purposes. Use the following use cases to ensure correct behavior.

### Basic test case
In this case, both weight values are valid, and all keys are in both lists. 
```json
{
  "salesWeight": 0.5,
  "stockWeight": 0.5,
  "productSales": [
    {"productId": "1", "sales": 50000},
    {"productId": "2", "sales": 100000},
    {"productId": "3", "sales": 100000},
    {"productId": "4", "sales": 75000}
  ],
  "productStock": [
    {"productId": "1", "stock": 100000},
    {"productId": "2", "stock": 400000},
    {"productId": "3", "stock": 200000},
    {"productId": "4", "stock": 300000}
  ]
}
```
Resulting list: [
  "2",
  "4",
  "3",
  "1"
].

### Invalid Weights test case
In this case, weight values are invalid, and all keys are in both lists. 
```json
{
  "salesWeight": 0,
  "stockWeight": 0,
  "productSales": [
    {"productId": "1", "sales": 50000},
    {"productId": "2", "sales": 100000},
    {"productId": "3", "sales": 100000},
    {"productId": "4", "sales": 75000}
  ],
  "productStock": [
    {"productId": "1", "stock": 100000},
    {"productId": "2", "stock": 400000},
    {"productId": "3", "stock": 200000},
    {"productId": "4", "stock": 300000}
  ]
}
```
Resulting error:
```json
{
  "type": "Weight format error",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "request": [
      "The sum of salesWeight and stockWeight must be equal to 1."
    ]
  }
}
```


### Invalid format
1. Required fields
    ```json
    {
      "salesWeight": 0,
      "stockWeight": 1,
      "productStock": [
        {"productId": "1", "stock": 100000},
        {"productId": "2", "stock": 400000},
        {"productId": "3", "stock": 200000},
        {"productId": "4", "stock": 300000}
      ]
    }
    ```

    Resulting error:
    ```json
    {
      "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
      "title": "One or more validation errors occurred.",
      "status": 400,
      "traceId": "00-cc97c0590e151d8e273ef4021807b18b-931f06008fc10de7-00",
      "errors": {
        "ProductSales": [
          "The ProductSales field is required."
        ]
      }
    }
    ```

2. Invalid type
    ```json
    {
        "salesWeight": 0,
        "stockWeight": 1,
        "productSales": [
          {"productId": "1", "sales": 50000},
          {"productId": "2", "sales": 100000},
          {"productId": "3", "sales": 100000},
          {"productId": "4", "sales": 75000}
        ],
        "productStock": [
          {"productId": "1", "stock": 100000.0},
          {"productId": "2", "stock": 400000},
          {"productId": "3", "stock": 200000},
          {"productId": "4", "stock": 300000}
        ]
    }
    ```

    Resulting error:
    ```json
    {
        "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        "title": "One or more validation errors occurred.",
        "status": 400,
        "traceId": "00-cc07dc4cabf35bd12f1eaad0eb5b16ad-1c197a326fa75898-00",
        "errors": {
          "request": [
            "The request field is required."
          ],
          "$.productStock[0].stock": [
            "The JSON value could not be converted to System.Int32. Path: $.productStock[0].stock | LineNumber: 10 | BytePositionInLine: 40."
          ]
        }
    }
    ```

### Complex data structures
1. The length of the sales list is less than the length of the stock list, it is assumed that the missing elements had no sales in the last 72 hours.
     ```json
        {
          "salesWeight": 0.5,
          "stockWeight": 0.5,
          "productSales": [
            {"productId": "1", "sales": 50000},
            {"productId": "3", "sales": 100000},
            {"productId": "4", "sales": 750000}
          ],
          "productStock": [
            {"productId": "1", "stock": 100000},
            {"productId": "2", "stock": 400000},
            {"productId": "3", "stock": 200000},
            {"productId": "4", "stock": 300000}
          ]
        }
      ```

    Resulting list: [
  "4",
  "2",
  "3",
  "1"
].

2. The length of the stock list is less than the length of the sales list. In this case, it is assumed that the missing elements are out of stock, so they are excluded from the listing (there is no need to show customers something that does not exist).
    ```json
        {
          "salesWeight": 0.5,
          "stockWeight": 0.5,
          "productSales": [
            {"productId": "1", "sales": 50000},
            {"productId": "2", "sales": 100000},
            {"productId": "3", "sales": 100000},
            {"productId": "4", "sales": 750000}
          ],
          "productStock": [
            {"productId": "1", "stock": 100000},
            {"productId": "3", "stock": 200000},
            {"productId": "4", "stock": 300000}
          ]
        }
      ```

    Resulting list: [
  "4",
  "3",
  "1"
].

## Running as container
1. **Installing dependencies**: Make sure you have Docker installed. If not, you can install it from the official website [here](https://www.docker.com/). 

2. **Build the docker image** from the _ProductSorting/dockerfile_:
   
``docker build -t product-sorting-service:latest . ``

3. **Run the container**:

`` docker run -d -p 8080:80 product-sorting-service ``

At this point, the service should be running in the container and ready to receive requests.

## Making requests to the container
1. **Install postman:** To send requests to the server, it is recommended to use Postman. You can install it from [here](https://www.postman.com/downloads/).
2. **Add the postman collection file** _SortingProducts.postman_collection.json_ as a new collection in your postman app, select _Sort Products_ request and send. 

