variable "region" {
  description = "AWS Region"
  type        = string
  default     = "ap-south-1" 
}

variable "ami_id" {
  description = "Ubuntu 22.04 LTS for ap-south-1"
  type        = string
  default     = "ami-07216ac99dc46a187" # Valid for Mumbai as of April 2026
}

variable "key_name" {
  description = "Name of the AWS Key Pair for SSH access"
  type        = string
  default     = "jenkins-key"
}

variable "jenkins_instance_type" {
  description = "Instance size for Jenkins (needs more RAM for builds)"
  type        = string
  default     = "t3.small"
}

variable "app_instance_type" {
  description = "Instance size for the deployment server"
  type        = string
  default     = "t3.micro"
}