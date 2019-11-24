# ReusableServices
 Services which can be used in another projects

## The link to the module: https://github.com/IuriiAksenov/ReusableServices. 

## Description
The purpose of the module is to send any messages (notification, alert, debug, etc) via two channels: email and vk-chat. Using RabbitMQ channel other services send command 

## Minimal requirements 
ASP.NET Core Runtime 2.2.1, .NET Core Runtime 2.2.1. The provided link is https://dotnet.microsoft.com/download/dotnet-core/2.2.
RabbitMQ 3.8.0. The provided link is https://www.rabbitmq.com/download.html.
(Optional) To send messages via Vk chat: vk group account with an access token.
(Optional) To send messages via email channel: email account.

## How to install
Download and install the necessary environment.
Download the repository.
Add ‘appsettings.json’ file and fill it with the real data. You can copy the appsettings.demo.json file that is given in repository and change its name.
Run the app to check whether it is working.
Then you can expand this service with new logic or use it as a single unit.

## Notes
Mail services (example, Google, Yahoo, Yandex) send emails only if the request was provided by security channel.
To send a message via RabbiMQ to “MailService” you have specified the channel name as “mails”. 
You can send mail via using the HTTP request to “MailController”.
