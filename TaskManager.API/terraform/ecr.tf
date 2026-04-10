resource "aws_ecr_repository" "task_manager_api" {
  name                 = "task-manager-api"
  image_tag_mutability = "MUTABLE"

  image_scanning_configuration {
    scan_on_push = true
  }
}

output "ecr_url" {
  value = aws_ecr_repository.task_manager_api.repository_url
}