# Resume-azure
My own resume hosted in azure ( cloud resume challenge)
will update soon

## Architecture
<img src=https://github.com/rghdrizzle/Resume-azure/blob/main/diagram.png>

## Cloud Resume Challenge
<p>This is a challenge where I build my resume on html and deploy it to the cloud with a proper workflow and cloud arhitecture. The Image above is the arhitecture of this challenge, we will be going through each process that I have been through and the challenges I have faced in this challenge.</p>

## Tools
<ul>
  <li>Azure functions</li>
  <li>Azure CDN</li>
  <li>Azure blob storage and Cosmos Db</li>
  <li>Terraform(IaC)</li>
  <li>Github Actions(CICD)</li>
</ul>

## Phase-1 (Building the Resume-Website)
<p>So the first phase was to build a resume static website using html and css. Since I didnt wanna spend too much time on desinging the resume website I used a template from the internet and altered the code depending on how I wanted it to look. Then I added all the details necessary for the resume page.</p>
<img src="https://github.com/rghdrizzle/Resume-azure/assets/92255903/7603d7e2-cf9b-4a25-869f-fe8fc693d688">
This is the result of the staic resume page.

## Phase-2 (Hosting the website in azure)
<p>This phase is all about deploying the static site to the cloud. In Azure we can achieve this by deploying the static site to a blob storage. Azure Blob storage has an option to configure it to host static websites. Once configured you can find a container named as $web. Now you can upload the website files directly from the portal or the cli. I personally chose to do it through the cli. The image below shows you the uploaded files for the static site in the blob storage</p>
<img src="https://github.com/rghdrizzle/Resume-azure/assets/92255903/95de4a72-b698-46c4-b4cb-01dfb0749bdd">
And you can access the static website through the primary endpoint given by azure for this particular site. [You can find it in the capabilites section in the overview of the Blob account and click "Static website" and find the primary and secondary endpoints]

## Phase-3 (Domain and CDN)
I bought a domain ( luqmaanrgh.me, yes pls do visit, thank you :) ) for this project. I could have used Azure DNS but my subscription credits wasnt able to actually get a domain name so I bought a domain someplace else. Then I pointed this domain to the Azure CDN endpoint. CDN refers to content delivery network. It is a network of multiple proxy servers with a primary goal of delivering content with high availability. So users across different geographical locations can access it faster.

## Phase-4 (Javascript-webapp)
Now we have a static website to display our resume. The next challenge was to create a counter to keep track of how many times the page has been visited and this can be achieved using js. I used an event listener to trigger and call the `getVisitcount()` function whenever the <a href="https://developer.mozilla.org/en-US/docs/Web/API/Document/DOMContentLoaded_event">dom elements or content gets reloaded</a>. This function sends a http request to the azure function( we will talk about it in the next phase) to get the count and display it in the website. You can take a look at the code to understand how it has been done. Its simply is a basic get request.

## Phase-5 (Azure functions)
Now comes the important part. I have to create a function to update the count of the visitors everytime the page is reloaded. This can be acheived by using HTTP trigger for the function. Which means the function will run everytime the page gets reloaded. So I created a function to fetch the count from the Azure cosmos DB( next phase) and then update the count by increasing it by one and then returning it to the front end so that it can display the count. 
```python:
for entity in entities:
        for key in entity.keys():
            if key == "count":
                updatedcounter = entity[key]
                entity[key]= updatedcounter+1
                table_client.update_entity(mode=UpdateMode.MERGE, entity=entity)
                response = {
                    "key": "count",
                    "value": updatedcounter
                }
```
The code above technically is an API which fetches the data from the database , iterates through the table and finds the required key , increases the value of that count and then updates it in the table and return it to the frontend. This function was written in python and I used the Azure SDK package for python to perfrom azure operations through code.

## Phase-6 (Azure Cosmos DB)
Now I have to create a cosmos Db to store the number of visitors. I had multiple options to choose from (graph.sql,table etc) and I chose table. The database itself is also serverless so I only had to pay for the usage. After creating the database , I created a table to enter the visiter count value. I used Table API to retrieve and update the count value. Since this was my frist time working with Table in Cosmos DB , It took me some time to understand how the data works and how the whole table is structured. This <a href="https://learn.microsoft.com/en-us/azure/cosmos-db/table/quickstart-python?tabs=azure-portal"> document</a> helped me understand how it works and I learnt how to do what I wanted in this project.

## Phase-7 (Terraform)
After creating the application with the necessary services , its time to automate the whole application. To automate the process of provisioning the resources in azure we can use terraform which is an IaC tool. Since in the previous phase I have already provisioned the resources using the cli and the portal I had to find a way to manage those resources.So I created an <a href="https://developer.hashicorp.com/terraform/language/import">import.tf file to create a state file for the existing resources</a> in the cloud provider. This was done by using the <a href="https://developer.hashicorp.com/terraform/language/import">import block</a>. After getting the state files I performed operations as how we would do normally while creating the resoruces from the start. After this I made the necessary changes needed to automate the provisioning such as storing the application setting variables. Coming to variables I stores few of them in the tfvars.tf file while for the variables that are meant to be a secret, I passed them as environmental variables( yes you can store the values of terraform variables as <a href="https://awstip.com/managing-secrets-on-terraform-71ed245a455f">env variables just make sure you give it a prefix like TF_VAR_<variable-name></a>) in the local machine, of cource there are many other ways to store and reference them but I personally chose this for this project. When it comes to automating the provisioning using CICD, I pass the values as secrets so the pipeline can use that to provision the resources with variable's values which are encrypted. Also for automating the provisioning using CICD, I had to store the state files in a backend. The backend used here is Terraform cloud. You can store it in any other vaults or backend. We will discuss about how the pipelines accesses those state files to provision the resources in the next Phase.

## Phase-8 (CI/CD)
Now to the CI/CD phase. The aim is to create pipelines for both backend and frontend. Whenever changes are made to the backend directory the cicd workflow will be triggered and executed. I separated the backend and terraform directories so if there are changes in either the pipeline will run and make necessary changes. The backend job will deploy the backend functions such as azure functions to the cloud with the changes I made , while the terraform job will provision the resources based on the changes made on the script. The frontend pipeline is focused on deploying the frontend to azure whenever changes are made to it too. To create a conditon based on if a pull request is made in a particular path was only possible due to the <a href="https://github.com/dorny/paths-filter">dorny/paths-filter</a> module for github actions. To authenticate and login into my azure account, <a href="https://learn.microsoft.com/en-us/azure/developer/github/connect-from-azure?tabs=azure-portal%2Cwindows#use-the-azure-login-action-with-a-service-principal-secret">I created a service principle for the pipeline to login and deploy the app</a>. We can improve the pipeline by adding tests for the azure functions which will be run everytime a push is made to the directory. Every required secret for backend job ,terraform and frontend job are stored as secrets in github actions. Creating a cicd pipeline definitely improve the development process so the developers can focus on the product instead of focusing on deployment and management of it. 
