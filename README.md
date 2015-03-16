### **Contract Reward Modifier**
[![][shield:support-ksp]][KSP:developers]&nbsp;
[![][shield:ckan]][CKAN:org]&nbsp;
[![][shield:license-mit]][CRMLicense]&nbsp;
[![][shield:license-cc-by-sa]][CRMLicense]&nbsp;
![][CRMFullWindow]

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

[DMagic]: http://forum.kerbalspaceprogram.com/members/59127
[TriggerAu]: http://forum.kerbalspaceprogram.com/members/59550

[CRMFullWindow]: http://i.imgur.com/FdXv5C1.jpg

[KSP:developers]: https://kerbalspaceprogram.com/index.php
[CKAN:org]: http://ksp-ckan.org/
[CRMLicense]: https://github.com/DMagic1/Contract-Modifier/blob/master/GameData/ContractRewardModifier/License.txt

[cconfig:release]: http://forum.kerbalspaceprogram.com/threads/101604
[toolbar:release]: http://forum.kerbalspaceprogram.com/threads/60863
[cwplus:release]: http://forum.kerbalspaceprogram.com/threads/91034

[shield:license-mit]: http://img.shields.io/:license-mit-a31f34.svg
[shield:license-cc-by-sa]: http://img.shields.io/badge/license-CC%20BY--SA-green.svg
[shield:support-ksp]: http://img.shields.io/badge/for%20KSP-v0.90-bad455.svg
[shield:ckan]: https://img.shields.io/badge/CKAN-Indexed-brightgreen.svg
[shield:support-toolbar]: http://img.shields.io/badge/works%20with%20Blizzy's%20Toolbar-1.7.8-7c69c0.svg
[shield:support-ccfg]: https://img.shields.io/badge/works%20with%20Contract%20Configurator-7.0-yellowgreen.svg
[shield:support-cwplus]: https://img.shields.io/badge/works%20with%20Contracts%20Window%20%2B-4.0-orange.svg