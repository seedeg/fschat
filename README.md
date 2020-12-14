# fs-chat

Prerequisites
 - Angular CLI needs to be installed (See https://cli.angular.io/)

Steps to run:
 - Open FsChat.sln
 - Compile and run FsChat.Services.APIs.Public. The API should be accessible at http://localhost:6001/. Perform a test API call, such as http://localhost:6001/api/testing/date to make sure that is running properly
 - Compile and run the FsChatUi by opening a command prompt and executing the following 2 commands:
	- npm install
	- ng serve
	- Angular app should be hosted on http://localhost:4200 by default