pipeline {
    agent any
    stages {
        stage('Build Docker Image') {
            when {
                branch 'master'
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
                branch 'master'
            }
            steps {
                docker stop sls_api
                docker rm sls_api
                docker container run -d --restart always --name sls_api -p '80:80' -p '443:443'  'sls_api:${env.BUILD_NUMBER}'
            }
        }
    }
}
