# # NopCommerce.Plugin.Payment.Barion

The Barion plugin enables integration between the **nopCommerce** system and the **Barion** payment gateway. [https://www.barion.com/](https://www.barion.com/)

# Supported operation

|           Operation     |Payment                       |
|----------------|-------------------------------|
|Immediate payment|  True          |          
|Reservation payment   |True|            |
|Refund          |True|
|Void          |True|
|Capture          |True|
|Recurring payment  |False  (will be add in next realease)|  


## Plugin Installation/Configuration 

Plugin use standart nopCommerce installation process. You can check also official installation guide [http://docs.nopcommerce.com/display/en/Plugins](http://docs.nopcommerce.com/display/en/Plugins)
or just build project and make sure that you copy output to plugins folder or you can download instalallation package from releases page or NopCommerce Store 
  
###	Configuration 
![Plugin configuration](https://raw.githubusercontent.com/TomasHorvath/NopCommerce.Plugin.Payment.Barion/master/img/configure.png)

### Test card

CARD 1 - payments with this card will always SUCCEED

BIN: 4444 8888 8888 5559
Expiration date: any future date
CVC: any 3-digit number
Test-card2.png

CARD 2 - payments with this card will always FAIL

BIN: 4444 8888 8888 4446
Expiration date: any future date
CVC: any 3-digit number

check barion sandbox testing  [https://docs.barion.com/Sandbox](https://docs.barion.com/Sandbox)  

##	Payment process 
![enter image description here](https://github.com/TomasHorvath/NopCommerce.Plugin.Payment.Barion/blob/master/img/apptest.gif?raw=true)
 
