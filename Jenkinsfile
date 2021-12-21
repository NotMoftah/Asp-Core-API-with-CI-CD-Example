pipeline {
    agent any
    stages {
        stage('Build Docker Image') {
            when {
                branch 'main'
            }
            steps {
                sh 'docker build -t sls_api:latest .'
            }
        }
        stage('Run Docker Image') {
            when {
                branch 'main'
            }
            steps {
                sh 'docker stop sls_api || echo \"No Running container\"'
                sh 'docker rm sls_api || echo \"No Existing container\"'
                sh 'docker container run -d --restart always --name sls_api -p 80:80 -p 443:443 sls_api:latest'
            }
        }
    }
}
