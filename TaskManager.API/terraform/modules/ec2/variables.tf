variable "vpc_id" {
  type = string
}

variable "public_subnet_id" {
  type = string
}

variable "jenkins_sg_id" {
  type = string
}

variable "app_sg_id" {
  type = string
}

variable "key_name" {
  type    = string
}

variable "ami_id" {
  type    = string
  default = "ami-07216ac99dc46a187"
}