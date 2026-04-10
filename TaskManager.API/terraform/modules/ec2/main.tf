resource "aws_instance" "jenkins" {
  ami                    = var.ami_id  # Use the variable here!
  instance_type          = "t3.small"
  subnet_id              = var.public_subnet_id
  vpc_security_group_ids = [var.jenkins_sg_id]
  key_name               = var.key_name
  user_data              = file("${path.root}/scripts/jenkins_setup.sh")

  tags = { Name = "Jenkins-Server" }
}

resource "aws_instance" "app_server" {
  ami                    = var.ami_id  # Use the variable here!
  instance_type          = "t3.micro"
  subnet_id              = var.public_subnet_id
  vpc_security_group_ids = [var.app_sg_id]
  key_name               = var.key_name
  user_data              = file("${path.root}/scripts/app_setup.sh")

  tags = { Name = "App-Server" }
}