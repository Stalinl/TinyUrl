# Welcome to the TinyUrl Service application

## What is this project?

This project contains Web api to convert long Url to Tiny Url and vise versa. There are is also a simple web application to visualize the functionalities.

## How do I get started?

* Clone via the Clone button in the code tab.
* Visual Studio:
  * Just press the clone button in the VSTS code tab

Choose and install one of these supported IDEs:

* [Visual Studio Code](https://code.visualstudio.com/Download)

## How do deploy

* Find the YAML script file in the repository named as azure-pipelines.yml
* Fill out all the variables correctly and run in Azure DevOps
* The Pipeline should provision the Sql database with required tables first
* Then the pipeline will deploy the web Api with applicaiton-insight enabled for monitoring purpose

## Deployment details

* Application is currently deployed in Azure. Please refer for links. It should be up and running.

|  	| Url |
|--- |--- |
| Web Api | https://mytinyurl.azurewebsites.net/api/v1/	|
| Web Application | https://mytiny2.azurewebsites.net/ |

## Code coverage of this application

* The solution has 87% code coverage for now.

![alt text](https://github.com/Stalinl/TinyUrl/tree/master/docs/CodeCoverage1.png)

![alt text](https://github.com/Stalinl/TinyUrl/tree/master/docs/CodeCoverage2.png)



Thanks for visiting!
