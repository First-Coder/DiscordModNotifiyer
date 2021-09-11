[![First-Coder](https://first-coder.de/images/logos/LogoFirstCoderDarkHorizontal.png)](https://first-coder.de/)

---

# Discord Mod Notifiyer

Discord Mod Notifiyer is a dotnet core application that will push a message on your discord server if a steam workshop mod is updated. It can automatically be executed and 
can also being used as service in Linux. 

## Installation of .Net Core 3.1

Install the following packages in the terminal

```bash
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
sudo apt install apt-transport-https
sudo apt install dotnet-runtime-3.1
sudo apt install screen
```

## Installation of the application itself
Move the DMN directory to /opt/DMN/.

## Installation of the systemctl service
Move the file DMN.service to the Systemd directory.

activate the autostart with the following command

```bash
systemctl enable LinuxProxyChanger
```

## Usage

Start programme manually
```bash
systemctl start DMN
```

Stop programme manually
```bash
systemctl stop DMN
```

Check programme status
```bash
systemctl status DMN
```

You can call up the console of the program with the following command

```bash
sudo screen -r DiscordModNotifiyer
```
## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[GPL-3.0](https://choosealicense.com/licenses/gpl-3.0/)

[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/donate?hosted_button_id=8PBF4BN7R46TE)
