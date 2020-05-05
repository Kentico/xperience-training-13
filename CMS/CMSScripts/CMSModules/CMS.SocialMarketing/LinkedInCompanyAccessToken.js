cmsdefine(['CMS/Core'], function (Core) {

    var Module = function (opt) {
        this.setData = function(parameters) {
            var companyId = $companyId.attr("value");
            
            if (!parameters.companies.length) {
                if (companyId) {
                    showMessage(data.msgNoCompanyAccess.replace(/\{0\}/g, $companyName.val()));
                } else {
                    showMessage(data.msgNoCompany);
                }

                return;
            }

            if (companyId) {
                // Access token is reauthorized
                var isUserCompanyAdmin = false;
                for (var i = 0; i < parameters.companies.length; i++) {
                    if (parameters.companies[i].ID == companyId) {
                        isUserCompanyAdmin = true;
                        break;
                    }
                }
                if (!isUserCompanyAdmin) {
                    showMessage(data.msgNoCompanyAccess.replace(/\{0\}/g, $companyName.val()));

                    return;
                }
            }

            $token.val(parameters.accessToken);
            $expiration.val(parameters.tokenExpiration);
            $expirationString.val(parameters.tokenExpirationString);
            $expirationInfo.html(data.msgExpiration.replace(/\{0\}/g, parameters.tokenExpirationString));
            $appId.val(parameters.tokenAppId);
            storeCompanies(parameters.companies);

            if (!companyId) {    
                showCompanies(parameters.companies);
            }
        }

        var core = new Core(opt, null),
            ctx = core.ctx,
            data = ctx.data,

            $token = $cmsj('#' + data.tokenControlId),
            $expiration = $cmsj('#' + data.tokenExpirationControlId),
            $expirationString = $cmsj('#' + data.tokenExpirationStringControlId),
            $expirationInfo = $cmsj('#' + data.tokenExpirationInfoControlId),
            $appId = $cmsj('#' + data.appIdControlId),
            $companyId = $cmsj('#' + data.companyIdControlId),
            $companyName = $cmsj('#' + data.companyNameControlId),
            $companySelector = $cmsj('#' + data.companyDropdownControlId),
            $companies = $cmsj('#' + data.companiesControlId),
            $infoLabel = $cmsj('#' + data.infoLabelControlId),
            $btnGetToken = $cmsj('#' + data.getTokenButtonId),


        showMessage = function (msg) {
            $infoLabel.html(msg);
        },


        storeCompanies = function(companies) {
            $companies.val(encodeURI(JSON.stringify(companies)));
        },


        getStoredCompanies = function() {
            return JSON.parse(decodeURI($companies.val()) || null);
        },

        companyChanged = function() {
            $companyId.val($companySelector.val());
            $companyName.val($companySelector.find('option:selected').text());
        },


        showCompanies = function(companies) {
            for (var i = 0; i < companies.length; i++) {
                var company = companies[i];
                $companySelector.append($cmsj('<option></option>').val(company.ID).text(company.Name));
            }
            $companySelector.removeClass('hide');
            $btnGetToken.addClass('hide');
        },


        init = function () {
            $companySelector.change(companyChanged);

            var companies = getStoredCompanies();
            if (companies && companies.length) {
                showCompanies(companies);
                $companySelector.val($companyId.val());
            }

            if ($expirationString.val()) {
                $expirationInfo.html(data.msgExpiration.replace(/\{0\}/g, $expirationString.val()));
            }
        };

        init();

        if (!window.linkedInCompanyControl) {
            window.linkedInCompanyControl = {};
        }
        window.linkedInCompanyControl[data.clientId] = this;
    };

    return Module;
});
