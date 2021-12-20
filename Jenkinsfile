pipeline {
    agent any
    stages {
        stage('Build Docker Image') {
            when {
                branch 'main'
            }
            steps {
                script {
                    app = docker.build("sls_api")
                    app.inside {
                        sh 'echo $(curl localhost:80/weatherforecast)'
                    }
                }
            }
        }
        stage('Run Docker Image') {
            when {
                branch 'main'
            }
            steps {
                sh 'docker rm sls_api'
                sh 'docker stop sls_api'
                sh 'docker container run -d --restart always --name sls_api -p 80:80 -p 443:443 sls_api:${env.BUILD_NUMBER}'
            }
        }
    }
}
