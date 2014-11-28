/*
Copyright (c) 2013, Colaboradores do INSTITUTO DE PESQUISAS ELDORADO
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided 
that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions and the 
following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the 
following disclaimer in the documentation and/or other materials provided with the distribution.

3. Neither the name of the INSTITUTO DE PESQUISAS ELDORADO nor the names of its contributors may be used to endorse or promote 
products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

// global variables of urls requests
    urlRooms = "http://requestserver/Rooms";
    urlRoomsId = "http://requestserver/RoomsId";
    urlDefault = "http://serverfiles/Get_A_Room_Files/";

// ready function
    $(document).ready(function() {

        setURL();

        setCidade();

        $(".sair").each(function(){
            this.setAttribute('onclick',"fechar()");
        });
	
		$("#HorarioInicioCPS").val(horarioNormal());  
		$($($("#HorarioInicioCPS").parents()[1]).find('span')[2]).text(horarioNormal());  
		
		$("#HorarioFinalCPS").val(horarioMaisUm());  
		$($($("#HorarioFinalCPS").parents()[1]).find('span')[2]).text(horarioMaisUm());  
		
		$("#HorarioInicioBSB").val(horarioNormal());  
		$($($("#HorarioInicioBSB").parents()[1]).find('span')[2]).text(horarioNormal());  
		
		$("#HorarioFinalBSB").val(horarioMaisUm());  
		$($($("#HorarioFinalBSB").parents()[1]).find('span')[2]).text(horarioMaisUm());  
		
		$("#HorarioInicioPOA").val(horarioNormal());  
		$($($("#HorarioInicioPOA").parents()[1]).find('span')[2]).text(horarioNormal());  
		
		$("#HorarioFinalPOA").val(horarioMaisUm());  
		$($($("#HorarioFinalPOA").parents()[1]).find('span')[2]).text(horarioMaisUm());  


		$("#HorarioInicioCPS").on('change', function() {
            invalidaEstados();
            var x;
            x = $("#HorarioInicioCPS").val();
            var hora;
            hora = parseInt(x.split(':')[0], 10) + 1;
            if(hora>23){
            hora=0;
            }
            if(hora>=00 && hora<10){
            hora = hora.toString();
            hora="0" + hora;
            }
            else{
            hora = hora.toString();
            }
            var minuto;
            minuto = x.split(':')[1];
            var ultima;
            ultima = hora+":"+minuto;
            $("#HorarioFinalCPS").val(ultima);  
            $($($("#HorarioFinalCPS").parents()[1]).find('span')[2]).text(ultima); 
		});
		
		$("#HorarioInicioBSB").on('change', function() {
            invalidaEstados();
            var x;
            x = $("#HorarioInicioBSB").val();
            var hora;
            hora = x[0] + x[1];
            hora = parseInt(hora, 10);
            hora=hora+1;
            if(hora>23){
            hora=0;
            }
            if(hora>=00 && hora<10){
            hora = hora.toString();
            hora="0" + hora;
            }
            else{
            hora = hora.toString();
            }
            var minuto;
            minuto = x[3] + x[4];
            var ultima;
            ultima = hora+":"+minuto;
            $("#HorarioFinalBSB").val(ultima);  
            $($($("#HorarioFinalBSB").parents()[1]).find('span')[2]).text(ultima); 
		});
		
		$("#HorarioInicioPOA").on('change', function() {
            invalidaEstados();
            var x;
            x = $("#HorarioInicioPOA").val();
            var hora;
            hora = x[0] + x[1];
            hora = parseInt(hora, 10);
            hora=hora+1;
            if(hora>23){
            hora=0;
            }
            if(hora>=00 && hora<10){
            hora = hora.toString();
            hora="0" + hora;
            }
            else{
            hora = hora.toString();
            }
            var minuto;
            minuto = x[3] + x[4];
            var ultima;
            ultima = hora+":"+minuto;
            $("#HorarioFinalPOA").val(ultima);  
            $($($("#HorarioFinalPOA").parents()[1]).find('span')[2]).text(ultima); 
		});

        $("#HorarioFinalCPS").change(function(){
            invalidaEstados();
        });
        $("#HorarioFinalBSB").change(function(){
            invalidaEstados();
        });
        $("#HorarioFinalPOA").change(function(){
            invalidaEstados();
        });
	});
	
	/**
	 * Refresh the status of all the room according to the last selected time range
	 */
	function atualizaEstados() {
		var horaInicio = $("#HorarioInicioCPS").val();
		var horaFim = $("#HorarioFinalCPS").val();
		
		// sets the last selected time
		$($($("#HorarioInicioCPS").parents()[1]).find('span')[2]).text(horaInicio);
		$("#HorarioInicioCPS").val(horaInicio);
		$($($("#HorarioFinalCPS").parents()[1]).find('span')[2]).text(horaFim);
		$("#HorarioFinalCPS").val(horaFim);
			
		solicitaEstados();
	}

    // Set url defaults from config file
    function setURL()
    {
        var url = urlDefault;
        
        //$.mobile.loading( 'show' ); 
        $.ajax({
            url: url,
            type: 'GET',
            complete: function(data){
                //$.mobile.loading( 'hide' ); //Hide spinner
                // get data.urlRooms
                // get data.urlRoomsId
                if (data.responseText.length > 0)
                {
                    var xml = $.parseXML(data.responseText)
                    urlRooms = $(xml).find("urlRooms").text();
                    urlRoomsId = $(xml).find("urlRoomsId").text();
                }
            }
        });

    }

    // Clear room status
    function invalidaEstados()
    {
        $(".ocupada").each(function()
        {
            $(this).addClass("inativa");
            $(this).attr('onclick', null);
            $(this).removeClass("ocupada");
        });
        $(".parcial").each(function()
        {
            $(this).addClass("inativa");
            $(this).attr('onclick', null);
            $(this).removeClass("parcial");
        });
        $(".livre").each(function()
        {
            $(this).addClass("inativa");
            $(this).attr('onclick', null);
            $(this).removeClass("livre");
        });
    }

	/**
	 * Opens the camera to read a QR code and redirect to confirmation screen
	 */
	function solicitarQrCode() {
		var scanner = cordova.require("cordova/plugin/BarcodeScanner");
		scanner.scan(
			function (result) {
				idSalaQrCode = result.text;
			
				solicitaEstadoQRCode(idSalaQrCode);
			}
		);
	}

	/**
	 * Scheduling of the room through QR code
	 */
	function agendaViaQrCode() {
		$.mobile.loading( 'show' );
		
		var horaInicio = $("#HorarioInicioCPS").val();
		var horaFim = $("#HorarioFinalCPS").val();
		
		agendaSala(idSalaQrCode, horaInicio, horaFim);
		
		$('#salaInfo').html(getDescSala(idSalaQrCode));
	}
	
	/**
	 * Returns to the rooms screen (with the updated status)
	 */
	function voltarSalas() {
		window.location.href = "GetARoom.html#";
		
		/* adds a delay to wait browser to change the components or else the loading 
		 * spinner would be called before and closed after the browser refresh, not being shown */
		setTimeout(function(){
			atualizaEstados();
		}, 100);		
	}

	/**
	 * Requests the status of the room to show in the confirmation dialog
	 */
	function solicitaEstadoQRCode(idSala) {
		$.mobile.loading( 'show' );

		cidade = window.localStorage.getItem("cidade");
        if (cidade == "#SalasCampinas") {
            $("#horaIni").val($("#HorarioInicioCPS").val());
            $("#horaFim").val($("#HorarioFinalCPS").val());
        }
        if (cidade == "#SalasBrasilia") {
            $("#horaIni").val($("#HorarioInicioBSB").val());
            $("#horaFim").val($("#HorarioFinalBSB").val());
        }
        if (cidade == "#SalasPortoAlegre") {
            $("#horaIni").val($("#HorarioInicioPOA").val());
            $("#horaFim").val($("#HorarioFinalPOA").val());
        }

        $.ajax({
            url: urlRooms,
            type: 'POST',
            data: 'tempo='+$("#tempo").val() +
            '&cidade='+cidade+
			'&horaIni='+$("#horaIni").val()+
            '&horaFim='+$("#horaFim").val(),
            dataType: "json",
            complete: function(data){
                $.mobile.loading( 'hide' );

                // fills each room with correspondent status
                $.each($.parseJSON(data.responseText), function(key,value){
					if (key == idSala) {
						$("#salaQrCode").removeClass('inativa');
						$("#salaQrCode").removeClass('ocupada');
						$("#salaQrCode").removeClass('parcial');
						$("#salaQrCode").removeClass('livre');
						$("#salaQrCode").attr('onclick', null);
						$("#salaQrCode").removeClass('ui-bar-e');
						$("#salaQrCode").addClass(value);

						var desc = getDescSala(key);
						$("#descQrCode").text(desc.split(' ')[0]);
						$("#letraQrCode").text(desc.split(' ')[1]);

						// if the room is not available, shows a message informing the user
						if (value == 'livre') {
							$("#sala_nao_disponivel").hide();
							$("#opcoes_confirmacao").show();
						} else {
							$("#sala_nao_disponivel").show();
							$("#opcoes_confirmacao").hide();
						}
							
						window.location.href = window.location.href.split('#')[0] + "#confirma_qr_code";
					}
                });
            }
        });
	}
	
    // Get status from rooms of a city
    function solicitaEstados()
    {
        $.mobile.loading( 'show' ); 

        // get city at local storage
        cidade = window.localStorage.getItem("cidade");

        if (cidade == "#SalasCampinas")
        {
            $("#horaIni").val($("#HorarioInicioCPS").val());
            $("#horaFim").val($("#HorarioFinalCPS").val());
        }
        if (cidade == "#SalasBrasilia")
        {
            $("#horaIni").val($("#HorarioInicioBSB").val());
            $("#horaFim").val($("#HorarioFinalBSB").val());
        }
        if (cidade == "#SalasPortoAlegre")
        {
            $("#horaIni").val($("#HorarioInicioPOA").val());
            $("#horaFim").val($("#HorarioFinalPOA").val());
        }

        url = urlRooms;
        $.ajax({
            url: url,
            type: 'POST',
            data: 'tempo='+$("#tempo").val() +
            '&cidade='+cidade+
            '&horaIni='+$("#horaIni").val()+
            '&horaFim='+$("#horaFim").val(),
            dataType: "json",
            complete: function(data){
                $.mobile.loading( 'hide' ); //Hide spinner

                // fills each room with correspondent status
                $.each($.parseJSON(data.responseText), function(key,value){
                    $($("div[id='estado"+key+"']").parents('div')[0]).removeClass('inativa');
                    $($("div[id='estado"+key+"']").parents('div')[0]).removeClass('ocupada');
					$($("div[id='estado"+key+"']").parents('div')[0]).removeClass('parcial');
					$($("div[id='estado"+key+"']").parents('div')[0]).removeClass('livre');
					$($("div[id='estado"+key+"']").parents('div')[0]).attr('onclick', null);
					$($("div[id='estado"+key+"']").parents('div')[0]).removeClass('ui-bar-e');
                    $($("div[id='estado"+key+"']").parents('div')[0]).addClass(value);
                });

                // add link to request a room
                $(".livre").each(function(){
                    $(this).attr('onclick',"solicitaSala(this)");
                });


            }
        });
    }

    // Set city parameter
    function setCidade()
    {
        if(window.localStorage.getItem("cidade")==null)
        {
            window.localStorage.setItem("cidade", "#SalasCampinas");
        }
        $("#cidade").val(window.localStorage.getItem("cidade"));
    }

    // Reload city
    function reCidade()
    {
        if(window.localStorage.getItem("cidade")!=null)
        {
            window.location.href = window.location.href.split('#')[0] + window.localStorage.getItem("cidade");
        }
        else
        {
            window.localStorage.setItem("cidade", "#SalasCampinas");

            window.location.href = window.location.href.split('#')[0] + window.localStorage.getItem("cidade");
		}
        $("#cidade").val(window.localStorage.getItem("cidade"));
        
    }
	
    // Clearing status from a city before changing
	function fechaCidades(cidade)
    {
        invalidaEstados();

        hash = '#' + cidade.href.split('#')[1]
        window.localStorage.setItem("cidade", hash);
        
        $("#cidade").val(hash);

        window.location.href = window.location.href.split('#')[0] + hash;
    }
	
    // Request a room
    function solicitaSala(sala)
    {
        if ($("#cidade").val() == "#SalasCampinas")
        {
            $("#horaIni").val($("#HorarioInicioCPS").val());
            $("#horaFim").val($("#HorarioFinalCPS").val());
        }
        if ($("#cidade").val() == "#SalasBrasilia")
        {
            $("#horaIni").val($("#HorarioInicioBSB").val());
            $("#horaFim").val($("#HorarioFinalBSB").val());
        }
        if ($("#cidade").val() == "#SalasPortoAlegre")
        {
            $("#horaIni").val($("#HorarioInicioPOA").val());
            $("#horaFim").val($("#HorarioFinalPOA").val());
        }

		agendaSala(sala.id, $("#horaIni").val(), $("#horaFim").val());
    }
	
	function agendaSala(salaId, horaIni, horaFim) {	
        url = urlRoomsId;
        $.ajax({
            url: url,
            type: 'POST',
            data: 'sala='+salaId +
                '&tempo='+$("#tempo").val() + '&user=' +
                window.localStorage.getItem("usuario") +
                '&horaIni='+horaIni+
                '&horaFim='+horaFim,
            beforeSend: function() { $.mobile.loading( 'show' ); }, //Show spinner
            complete: function(data){
                $.mobile.loading( 'hide' ); //Hide spinner

                // room free
                if($.parseJSON(data.responseText).estado == "livre"){
                    window.location.href = window.location.href.split('#')[0] + "#reservou";
                }
                // room busy
                else {
                    window.location.href = window.location.href.split('#')[0] + "#nreservou";
                }
            }
        });
	}
	
    // Set city parameter 
	function solicitaCidade(cidade)
    {
		var idcidade = jQuery(cidade).find("a").attr("href");
        window.localStorage.setItem("cidade", idcidade);

		if(idcidade == "campinas"){
			window.localStorage.setItem("cidade", "#SalasCampinas");
		}
		if(idcidade == "brasilia"){
			window.localStorage.setItem("cidade", "#SalasBrasilia");
		}
		if(idcidade == "portoAlegre"){
			window.localStorage.setItem("cidade", "#SalasPortoAlegre");
        }
    }
	
    // Reload page
    function reloadStatus()
    {
        window.location.href = window.location.href.split('#')[0] + '#' +
            window.location.href.split('?')[1];
        location.reload();
    }
	
    // Logout
	function fechar()
    {
		localStorage.clear();
		window.location.href =  'login.html' ;
    }
	
    document.addEventListener("deviceready", onDeviceReady, false);

    // PhoneGap is loaded and it is now safe to make calls PhoneGap methods
    function onDeviceReady() {
        // Register the event listener
        document.addEventListener("backbutton", onBackKeyDown, false);
    }

    // Handle the back button
    function onBackKeyDown() {
        navigator.app.exitApp();
    }

    // get start meeting time
    function horarioMaisUm()
    {
		var d = new Date();
		var x = d.getHours()+1;
		var y = d.getMinutes();
		var horarioFim = 2;
		
        if(y == 0){
			y = 0 + "0";
		}
		if(y > 0 && y <= 15){
            y = 15;
		}
		if(y > 15 && y <= 30){
            y = 30;
		}
		if(y > 30 && y <= 45){
            y = 45;
		}
		if(y > 45 && y <= 59){
            x = x + 1;
			y = "00";
		}
        horarioFim = x + ":" + y;
		
		return horarioFim;
    }
	
    // get end meeting time
	function horarioNormal()
    {
		var d = new Date();
		var x = d.getHours();
		var y = d.getMinutes();
	
		var horarioIni = 2;
		
        if(y == 0){
			y = 0 + "0";
		}
		if(y > 0 && y <= 15){
            y = 15;
		}
		if(y > 15 && y <= 30){
            y = 30;
		}
		if(y > 30 && y <= 45){
            y = 45;
		}
		if(y > 45 && y <= 59){
            x = x + 1;
			y = "00";
		}

        horarioIni = x + ":" + y;

		return horarioIni;
    }

    // onscroll to handle #hashlinks
    window.onscroll = function (e) 
    {
        window.onscroll = function(e){e.preventDefault();}
    }

	// used to store the ID of the room which will be scheduled
	var idSalaQrCode;

	// maps the description with the ID of the room
	var salasIdDesc = {"salaReuniaoC":"REUNI\u00c3O C", "salaReuniaoD":"REUNI\u00c3O D", "salaApoio03":"APOIO 3", "salaApoio04":"APOIO 4", "salaApoio05":"APOIO 5", "salaApoio06":"APOIO 6", "salaApoio10":"APOIO 10"};

	// returns the description of the room with the received ID
	function getDescSala(idSala){
		return salasIdDesc[idSala];
	}