# Firebase Hosting Deployment with Terraform

This directory contains Terraform configuration to deploy the PeakyFlow Blazor WebAssembly app to Google Firebase Hosting.

## Prerequisites

1. **Google Cloud Project**: Create a new GCP project or use existing one
2. **Google Cloud CLI**: Install and authenticate with `gcloud auth login`
3. **Terraform**: Install Terraform >= 1.0
4. **Firebase CLI**: Install Firebase CLI for deployment

## Setup Steps

### 1. Configure Variables
```bash
cp terraform.tfvars.example terraform.tfvars
# Edit terraform.tfvars with your project details
```

### 2. Initialize Terraform
```bash
terraform init
```

### 3. Plan Infrastructure
```bash
terraform plan
```

### 4. Apply Infrastructure
```bash
terraform apply
```

### 5. Build and Deploy App
```bash
# Build the Blazor WASM app
cd ../src/PeakyFlow.Aspire/PeakyFlow.Aspire.Mobile
dotnet publish -c Release

# Deploy to Firebase Hosting
cd ../../../terraform
firebase deploy --only hosting
```

## Configuration Files

- `main.tf` - Main Terraform configuration
- `variables.tf` - Input variables
- `outputs.tf` - Output values
- `firebase.json` - Firebase Hosting configuration
- `terraform.tfvars` - Your project-specific values (create from example)

## Resources Created

- Firebase project initialization
- Firebase Hosting site
- Required Google Cloud APIs

## URLs

After deployment, your app will be available at:
- `https://[site-id].web.app`
- `https://[site-id].firebaseapp.com`
