### **Contract Reward Modifier**
[![][shield:support-ksp]][KSP:developers]&nbsp;
[![][shield:ckan]][CKAN:org]&nbsp;
[![][shield:license-mit]][CRMLicense]&nbsp;
[![][shield:license-cc-by-sa]][CRMLicense]&nbsp;
![][CRM:FullWindow]

[![][shield:support-toolbar]][toolbar:release]&nbsp;
[![][shield:support-ccfg]][cconfig:release]&nbsp;
[![][shield:support-cwplus]][cwplus:release]&nbsp;


### People, and Info
-------------------------------------------

#### Authors and Contributors

[DMagic][DMagic]: Author and maintainer

[TriggerAu][TriggerAu]: Contract Reward Modifier uses a version of TriggerAu's KSP Plugin Framework

#### License

The code is released under the [MIT license][CRMLicense]; all art assets are released under the [CC-BY-SA license][CRMLicense]

#### FAQ

  * What does Contract Reward Modifier do?
     * It allows you to customize the reward and penalty values for each contract and parameter type.
	 * You can limit the number of contracts of each type offered; or prevent any from appearing.
	 * All values can be set and modified through an in-game user interface.
  * Does CRM support contracts from other addons?
     * All contract and parameter types are accessed upon startup; any types from other addons will be detected.
	 * Contract Configurator contracts require some special handling and cannot be completely blocked, but are otherwise supported.
  * Can values be set for each save file?
     * Yes. Default values are set using the included config file (which can be changed and saved in-game), but any save file can set its own reward and penalty values; these will supercede values from the config file.
  * I don't want another icon cluttering up the toolbar or an in-game interface, can this still be used?
     * Yes. The toolbar icons can be completely removed (a field in the config file can be used to reset them); the values can be set in-game to your liking, then you can remove the toolbar icons.
	 * There is another option, only available in the config file, that will disable all save-specific settings. All values will be set entirely using the config file.
	 
### Usage instructions
------------------------------------------

The Contract Reward Modifier window presents a set of sliders that allow for changes to all contract and parameter reward amounts, as well as several other options.

#### Contract and Parameter Type Selection
------------------------------------------

![][CRM:DropDowns]

  * At the top of the window are two drop down buttons that allow for the selection of different contract and parameter types.
    * A special **Global Settings** selection is available for both Contract and Parameter Types.
    * Values can be set with the sliders under the global settings; these will be applied to all Contract/Parameter Types after pushing the **Apply To All** button.
  * All addon contract and parameter types should be available here.
  * Contract Configurator types are pushed to the bottom of the list to avoid any potential spoilers revealed by contract names.
  
  
#### Sliders
---------------------------------------------

#### Rewards and Penalties sliders
![][CRM:Slider-reward]

  * The sliders below control the amount given for each different reward and penalty type.
  * Strategy values are not directly affected by these sliders, i.e. setting science to 0.1% won't affect the science rewards given while using the science strategy.
  * The sliders are half-way log-scale; the left half of the slider allows for 0.1 - 100% of the original value, while the right half allows for 100-1000% of the original.
  
#### Duration slider
![][CRM:Slider-duration]

  * Contract duration can be adjusted from 10-1000%, but will only affect newly offered contracts.
  
#### Contract limit sliders
![][CRM:Slider-limits]

  * The maximum number of offered and active contracts of a type can be specified; any contracts beyond this amount will be rejected when the system tries to offer new contracts.
  * All Contract Configurator contracts are can only be set to allow 1 or more contracts to be offered.

#### Additional Settings
-----------------------------------------

#### Allow for 0% values
![][CRM:Warn-zero]

  * By default the values only go down to 0.1%; there is an option to allow for nearly 0% (0.00000001%) using the **Allow 0% Values** toggle.
    * Using this may cause odd rounding errors and prevent further adjustments from being made to any active contracts/parameters of that type.

#### Alter active contracts
![][CRM:Warn-alteractive]

  * A toggle option is available to **Alter Active Contracts** as well as newly offered contracts.
    * Turning this on and off while adjusting values may cause problems with reward values.
    * Be default it is off; but you should stick with either turning it on, or leaving it off; don't go back and forth.
 
#### Disable toolbars
![][CRM:Warn-toolbar]

  * There is also toggle to completely **Disable All Toolbars**.
    * This can be used if you have set the values you want and don't need the toolbar icon taking up space.
  * Toolbars can be re-enabled by changing the **disableToolbar** field to **True** at the top of the config file. 
  
  * If [Blizzy78's Toolbar][toolbar:release] is installed you can switch between it and the stock app launcher using the **Use Stock Toolbar** toggle.

####Reset Contract and Parameter values to default
![][CRM:Reset-contract]
![][CRM:Reset-param]

  * Contract/Parameter amounts can be reverted to the default values set in your config file using the **Reset Values** button.

#### Save values to a global config file
![][CRM:Warn-save]
  
  * The current values can be saved to the global config file in your GameData folder using the **Save To Config** button.
    * Values saved to the config file will serve as the default for current save games.
    * New save files (or save files that haven't been loaded while Contract Reward Modifier has been installed) will set all contract and parameter values using the config file settings.
  * The config file-only **disableSaveSpecificValues** field allows you to bypass any values set in your save files; all values stored in the save files will be deleted.


[DMagic]: http://forum.kerbalspaceprogram.com/members/59127
[TriggerAu]: http://forum.kerbalspaceprogram.com/members/59550

[CRM:FullWindow]: http://i.imgur.com/FdXv5C1.jpg
[CRM:DropDowns]: http://i.imgur.com/KmoIB5P.jpg?1
[CRM:Slider-reward]: http://i.imgur.com/OTtx8q7.jpg?1
[CRM:Slider-limits]: http://i.imgur.com/qBOKqLb.jpg?1
[CRM:Slider-duration]: http://i.imgur.com/k2PJ0LS.jpg?1
[CRM:Warn-zero]: http://i.imgur.com/ZxSEPjB.jpg?1
[CRM:Warn-alteractive]: http://i.imgur.com/KTrAboK.jpg?1
[CRM:Warn-toolbar]: http://i.imgur.com/v0k6BMW.jpg?1
[CRM:Reset-contract]: http://i.imgur.com/B1wir2M.jpg?1
[CRM:Reset-param]: http://i.imgur.com/PxYzX7x.jpg?1
[CRM:Warn-save]: http://i.imgur.com/tveX99k.jpg?1 

[KSP:developers]: https://kerbalspaceprogram.com/index.php
[CKAN:org]: http://ksp-ckan.org/
[CRMLicense]: https://github.com/DMagic1/Contract-Modifier/blob/master/GameData/ContractRewardModifier/License.txt

[cconfig:release]: http://forum.kerbalspaceprogram.com/threads/101604
[toolbar:release]: http://forum.kerbalspaceprogram.com/threads/60863
[cwplus:release]: http://forum.kerbalspaceprogram.com/threads/91034

[shield:license-mit]: http://img.shields.io/badge/license-mit-a31f34.svg
[shield:license-cc-by-sa]: http://img.shields.io/badge/license-CC%20BY--SA-green.svg
[shield:support-ksp]: http://img.shields.io/badge/for%20KSP-v1.0.5-bad455.svg
[shield:ckan]: https://img.shields.io/badge/CKAN-Indexed-brightgreen.svg
[shield:support-toolbar]: http://img.shields.io/badge/works%20with%20Blizzy's%20Toolbar-1.7.x-7c69c0.svg
[shield:support-ccfg]: https://img.shields.io/badge/works%20with%20Contract%20Configurator-1.x-yellowgreen.svg
[shield:support-cwplus]: https://img.shields.io/badge/works%20with%20Contracts%20Window%20%2B-5.x-orange.svg
