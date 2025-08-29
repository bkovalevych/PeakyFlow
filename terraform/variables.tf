variable "project_id" {
  description = "The Google Cloud Project ID"
  type        = string
}

variable "region" {
  description = "The Google Cloud region"
  type        = string
  default     = "us-central1"
}

variable "site_id" {
  description = "The Firebase Hosting site ID"
  type        = string
  default     = "peakyflow"
}
