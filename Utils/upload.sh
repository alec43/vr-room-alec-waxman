#!/usr/bin/env bash
set -e

# Load AWS secrets
if [ -f aws_secrets.env ]; then
  source aws_secrets.env
else
  echo "Error: aws_secrets.env not found"
  exit 1
fi

# Check that the required arguments have been provided
if [ "$#" -ne 3 ]; then
  echo "Usage: $0 <bucket_arn> <distribution_arn> <webgl_project_path>"
  exit 1
fi

# Parse the arguments
bucket_arn="$1"
distribution_arn="$2"
webgl_project_path="$3"
# aws_credentials_path="aws_secrets.env"

# Extract the bucket and distribution IDs from the ARNs
bucket_id=$(echo "$bucket_arn" | cut -d':' -f 5)
distribution_id=$(echo "$distribution_arn" | cut -d'/' -f 2)

# Validate the webgl project path
if [ ! -d "$webgl_project_path" ]; then
  echo "Error: $webgl_project_path is not a directory"
  exit 1
fi

# Configure the AWS CLI with the provided credentials
# aws configure --profile webgl-project < "$aws_credentials_path"
# export AWS_SECRET_ACCESS_KEY=xxx
# export AWS_ACCESS_KEY_ID=xxx

# Delete all existing objects in the S3 bucket
echo "Deleting all objects in s3://$bucket_id..."
aws s3 rm --recursive "s3://$bucket_id"

# Upload the webgl project to the S3 bucket
echo "Uploading $webgl_project_path to s3://$bucket_id"
aws s3 sync "$webgl_project_path" "s3://$bucket_id" --include "*" --exclude "*.unityweb" --exclude "*.js"

# Set different metadata for unityweb vs js files
echo "Setting metadata for unityweb files"
aws s3 sync "$webgl_project_path" "s3://$bucket_id" --exclude "*" --include "*.unityweb" --content-encoding br --content-type application/octet-stream --metadata-directive REPLACE

echo "Setting metadata for js files"
aws s3 sync "$webgl_project_path" "s3://$bucket_id" --exclude "*" --include "*.js" --content-encoding .wasm --content-type application/wasm --metadata-directive REPLACE

# Refresh the CloudFront distribution
echo "Refreshing CloudFront distribution $distribution_id"
invalidation_id=$(aws cloudfront create-invalidation --distribution-id "$distribution_id" --paths "/*" --query "Invalidation.Id" --output text)

# Check the status of the invalidation and log it
echo "Waiting for CloudFront invalidation to complete..."
status=""
while [ "$status" != "Completed" ]
do
  sleep 2
  status=$(aws cloudfront get-invalidation --distribution-id "$distribution_id" --id "$invalidation_id" --query "Invalidation.Status" --output text)
  echo "Current invalidation status: $status"
done

echo "Upload complete."
upload.sh
Displaying upload.sh.Previous