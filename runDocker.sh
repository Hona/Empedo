echo '-= Stopping empedo-production Container =-'
docker container stop empedo-production

echo '-= Removing Old template-bot-production Container =-'
docker container rm empedo-production

echo '-= Building Docker Image from Dockerfile ='
docker build -t empedo -f ./src/Empedo/Dockerfile .

echo '-= Runnning the Image ='
docker run -e "ENVIRONMENT=DEVELOPMENT" -v $PWD/PRODUCTION:/app/PRODUCTION -v $PWD/DEVELOPMENT:/app/DEVELOPMENT --network host --restart on-failure:5 --name "empedo-production" -d empedo