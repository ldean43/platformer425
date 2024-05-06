#!/bin/bash

# Function to recursively delete .wal files
delete_wal_files() {
    local dir="$1"
    # Loop through all files and directories in the given directory
    for file in "$dir"/*; do
        if [[ -d "$file" ]]; then
            # If it's a directory, recursively call the function on it
            delete_wal_files "$file"
        elif [[ -f "$file" && "${file##*.}" == "tga" ]]; then
            # If it's a file with .wal extension, delete it
            echo "Deleting $file"
            rm "$file"
        fi
    done
}

# Start the deletion process from the current directory
delete_wal_files "$PWD"
