# SlsApi
# Docker Useful commands
## Prepare, Run, then go interactive with docker container
```sh
docker build -t sls_api:latest .
docker container run -d --name sls_api -p 5000:80 -p 5001:443 sls_api:latest
docker exec -it sls_api bash
```
## Stop, Remove container & Remove image
```sh
docker stop sls_api
docker rm sls_api
docker image rm sls_api:latest
```


# Infrastructure Useful Commands
## Generate & Import sls key to aws (You need to execute this only one time per region)
```sh
ssh-keygen -b 2048 -t rsa -f ~/.ssh/slskey -q -N ""
aws ec2 import-key-pair --key-name "slskey" --public-key-material fileb://~/.ssh/slskey.pub
```
## Runway Infrastructure Deploy & Destroy
```sh
DEPLOY_ENVIRONMENT=dev runway deploy --ci
DEPLOY_ENVIRONMENT=dev runway destroy --ci
``` 