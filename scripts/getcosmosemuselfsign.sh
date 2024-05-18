#!/bin/bash
set -e

curl -k https://localhost:8081/_explorer/emulator.pem > ~/emulatorcert.crt
cp ~/emulatorcert.crt /usr/local/share/ca-certificates/.
sudo cp ~/emulatorcert.crt /usr/local/share/ca-certificates/.
sudo update-ca-certificates
