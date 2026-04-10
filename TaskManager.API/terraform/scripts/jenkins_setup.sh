#!/bin/bash

set -e

export DEBIAN_FRONTEND=noninteractive

# Update system
sudo apt update -y
sudo apt upgrade -y

# Install dependencies
sudo apt install -y fontconfig openjdk-21-jre wget curl gnupg

# Create keyring directory
sudo mkdir -p /usr/share/keyrings

# Add Jenkins repository key (2026 key)
curl -fsSL https://pkg.jenkins.io/debian-stable/jenkins.io-2026.key | sudo tee \
/usr/share/keyrings/jenkins-keyring.asc > /dev/null

# Add Jenkins repository
echo "deb [signed-by=/usr/share/keyrings/jenkins-keyring.asc] \
https://pkg.jenkins.io/debian-stable binary/" | sudo tee \
/etc/apt/sources.list.d/jenkins.list > /dev/null

# Update again
sudo apt update -y

# Install Jenkins
sudo apt install -y jenkins

# Install Docker
sudo apt install -y docker.io

# Start and enable Docker
sudo systemctl enable docker
sudo systemctl start docker

# Add users to docker group
sudo usermod -aG docker ubuntu
sudo usermod -aG docker jenkins

# Enable Jenkins
sudo systemctl enable jenkins
sudo systemctl start jenkins

# Restart Jenkins so docker group works
sudo systemctl restart jenkins