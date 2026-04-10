pipeline {
    agent any
    
    environment {
        AWS_ACCOUNT_ID = "158582149043"
        AWS_DEFAULT_REGION = "ap-south-1" 
        IMAGE_REPO_NAME = "task-manager-api"
        IMAGE_TAG = "${env.BUILD_NUMBER}"
        REPOSITORY_URL = "${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_DEFAULT_REGION}.amazonaws.com/${IMAGE_REPO_NAME}"
        APP_SERVER_IP = "43.205.196.14"
    }

    stages {
        stage('Checkout') {
            steps {
                git branch: 'main', url: 'https://github.com/SaloniPawar-59/TaskManager.API.git'
            }
        }

        stage('Unit Tests') {
            steps {
                script {
                    echo "Running xUnit Tests from Solution Root..."
                    // No need for ../ anymore because we are in the root!
                    sh "/usr/bin/dotnet test ../TaskManager.sln --configuration Release"
                }
            }
        }

        stage('Docker Build') {
            steps {
                script {
                    // We go into the API folder to find the Dockerfile and source code
                    dir('TaskManager.API') {
                        sh "docker build --pull -t ${REPOSITORY_URL}:${IMAGE_TAG} ."
                        sh "docker tag ${REPOSITORY_URL}:${IMAGE_TAG} ${REPOSITORY_URL}:latest"
                    }
                }
            }
        }

        stage('Push to ECR') {
            steps {
                script {
                    withCredentials([[$class: 'AmazonWebServicesCredentialsBinding', credentialsId: 'aws-credentials']]) {
                        sh "aws ecr get-login-password --region ${AWS_DEFAULT_REGION} | docker login --username AWS --password-stdin ${REPOSITORY_URL}"
                        sh "docker push ${REPOSITORY_URL}:${IMAGE_TAG}"
                        sh "docker push ${REPOSITORY_URL}:latest"
                    }
                }
            }
        }

        stage('Deploy to App Server') {
            steps {
                script {
                    sshagent(['app-server-ssh-key']) {
                        // Copying the compose file from its specific subdirectory
                        sh "scp -o StrictHostKeyChecking=no TaskManager.API/docker-compose.yml ubuntu@${APP_SERVER_IP}:/home/ubuntu/"

                        sh """
                        ssh -o StrictHostKeyChecking=no ubuntu@${APP_SERVER_IP} '
                            aws ecr get-login-password --region ${AWS_DEFAULT_REGION} | docker login --username AWS --password-stdin ${REPOSITORY_URL}
                            docker-compose pull
                            docker-compose up -d
                            docker image prune -f
                        '
                        """
                    }
                }
            }
        }
    }
    
    post {
        always {
            sh "docker rmi ${REPOSITORY_URL}:${IMAGE_TAG} ${REPOSITORY_URL}:latest || true"
        }
        failure {
            echo "Deployment failed. Check the logs and verify App Server connectivity."
        }
    }
}