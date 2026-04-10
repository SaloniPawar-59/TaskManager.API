module "network" {
  source = "./modules/vpc"
}

module "compute" {
  source            = "./modules/ec2"
  vpc_id            = module.network.vpc_id
  public_subnet_id  = module.network.public_subnet_id
  jenkins_sg_id     = module.network.jenkins_sg_id
  app_sg_id         = module.network.app_sg_id
  key_name          = var.key_name
  ami_id            = var.ami_id
}