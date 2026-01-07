document.addEventListener('DOMContentLoaded', function() {

  // Use buttons to toggle between views
  document.querySelector('#inbox').addEventListener('click', () => load_mailbox('inbox'));
  document.querySelector('#sent').addEventListener('click', () => load_mailbox('sent'));
  document.querySelector('#archived').addEventListener('click', () => load_mailbox('archive'));
  document.querySelector('#compose').addEventListener('click', () => compose_email('', '', ''));

  // By default, load the inbox
  load_mailbox('inbox');
});


function compose_email(initbody, initsubject, initrecip) {

  // Show compose view and hide other views
  document.querySelector('#emails-view').style.display = 'none';
  document.querySelector('#compose-view').style.display = 'block';

  // Clear out composition fields
  document.querySelector('#compose-recipients').value = initrecip;
  document.querySelector('#compose-subject').value = initsubject;
  document.querySelector('#compose-body').value = initbody;

  document.querySelector('#compose-form').onsubmit = () => {
    fetch('/emails', {
      method: 'POST',
      body: JSON.stringify({
          recipients: document.querySelector('#compose-recipients').value,
          subject: document.querySelector('#compose-subject').value,
          body: document.querySelector('#compose-body').value
      })
    })
    .then(response => response.json())
    .then(result => {
        console.log(result);
    });
  };
}


function load_mailbox(mailbox) {

  // Show the mailbox and hide other views
  document.querySelector('#emails-view').style.display = 'block';
  document.querySelector('#compose-view').style.display = 'none';

  // Show the mailbox name
  document.querySelector('#emails-view').innerHTML = `<h3 style="padding-bottom: 13px">${mailbox.charAt(0).toUpperCase() + mailbox.slice(1)}</h3>`;

  if (mailbox === 'inbox'){
    fetch('/emails/inbox')
    .then(response => response.json())
    .then(emails => {
        console.log(emails);
        for (let email of emails) {
            if(!email.read) {
                const element = document.createElement('button');

                element.style.border = '2px solid black';
                element.style.fontSize = '18px';
                element.style.width = '1050px';
                element.style.textAlign = 'left';
                element.style.backgroundColor = '#d1d1d1'

                element.innerHTML = '<strong style="font-size: 23px; padding-left: 6px; padding-right: 10px;">' + email.sender + '</strong>' + email.subject + '<div style="text-align: right; color: gray; padding-right: 4px;">' + email.timestamp + '</div>';

                element.addEventListener('click', () => load_email(email.id))
                document.querySelector('#emails-view').append(element);
            }
            else {
                const element = document.createElement('button');

                element.style.border = '2px solid black';
                element.style.fontSize = '18px';
                element.style.width = '1050px';
                element.style.textAlign = 'left';
                element.style.backgroundColor = 'white'

                element.innerHTML = '<strong style="font-size: 23px; padding-left: 6px; padding-right: 10px;">' + email.sender + '</strong>' + email.subject + '<div style="text-align: right; color: gray; padding-right: 4px;">' + email.timestamp + '</div>';

                element.addEventListener('click', () => load_email(email.id))
                document.querySelector('#emails-view').append(element);
            }
        }
    });
  }

  else if (mailbox === 'sent'){
    fetch('/emails/sent')
    .then(response => response.json())
    .then(emails => {
        console.log(emails);
        for (let email of emails) {
            if(!email.read) {
                const element = document.createElement('button');

                element.style.border = '2px solid black';
                element.style.fontSize = '18px';
                element.style.width = '1050px';
                element.style.textAlign = 'left';
                element.style.backgroundColor = '#d1d1d1'

                element.innerHTML = '<strong style="font-size: 23px; padding-left: 6px; padding-right: 10px;">' + email.sender + '</strong>' + email.subject + '<div style="text-align: right; color: gray; padding-right: 4px;">' + email.timestamp + '</div>';

                element.addEventListener('click', () => load_email(email.id))
                document.querySelector('#emails-view').append(element);
            }
            else{
                const element = document.createElement('button');

                element.style.border = '2px solid black';
                element.style.fontSize = '18px';
                element.style.width = '1050px';
                element.style.textAlign = 'left';
                element.style.backgroundColor = 'white'

                element.innerHTML = '<strong style="font-size: 23px; padding-left: 6px; padding-right: 10px;">' + email.sender + '</strong>' + email.subject + '<div style="text-align: right; color: gray; padding-right: 4px;">' + email.timestamp + '</div>';

                element.addEventListener('click', () => load_email(email.id))
                document.querySelector('#emails-view').append(element);
            }
        }
    });
  }

  else if (mailbox === 'archive'){
    fetch('/emails/archive')
    .then(response => response.json())
    .then(emails => {
        console.log(emails);
        for (let email of emails) {
            const element = document.createElement('button');

            element.style.border = '2px solid black';
            element.style.fontSize = '18px';
            element.style.width = '1050px';
            element.style.textAlign = 'left';
                element.style.backgroundColor = '#d1d1d1'

            element.innerHTML = '<strong style="font-size: 23px; padding-left: 6px; padding-right: 10px;">' + email.sender + '</strong>' + email.subject + '<div style="text-align: right; color: gray; padding-right: 4px;">' + email.timestamp + '</div>';

            element.addEventListener('click', () => load_email(email.id))
            document.querySelector('#emails-view').append(element);
        }
    });
  }
}


function load_email(id){
    fetch('/emails/' + id, {
        method: 'PUT',
        body: JSON.stringify({
            read: false
        })
    })
    fetch('/emails/' + id)
    .then(response => response.json())
    .then(email => {
        console.log(email);
        document.querySelector('#emails-view').innerHTML = '<strong>From: </strong>' + email.sender + '<br>';
        document.querySelector('#emails-view').innerHTML += '<strong>To: </strong>' + email.recipients + '<br>';
        document.querySelector('#emails-view').innerHTML += '<strong>Subject: </strong>' + email.subject + '<br>';
        document.querySelector('#emails-view').innerHTML += '<strong>Timestamp: </strong>' + email.timestamp + '<br>';

        const element = document.createElement('button');
        element.style.borderRadius = '7px'
        element.innerHTML = 'Reply'
        element.addEventListener('click', () => compose_email("On " + email.timestamp + " " + email.sender + " wrote: " + email.body, "Re: " + email.subject, "" + email.sender))
        document.querySelector('#emails-view').append(element);

        if (!email.archived){
            const element = document.createElement('button');
            element.style.borderRadius = '7px'
            element.innerHTML = 'Archive'
            element.addEventListener('click', () => fetch('/emails/' + id, {
                method: 'PUT',
                body: JSON.stringify({
                    archived: true
                })
            }).then(() => {
                load_mailbox('inbox')
            }))
            document.querySelector('#emails-view').append(element);
        }
        else{
            const element = document.createElement('button');
            element.style.borderRadius = '7px'
            element.innerHTML = 'Unarchive'
            element.addEventListener('click', () => fetch('/emails/' + id, {
                method: 'PUT',
                body: JSON.stringify({
                    archived: false
                })
            }).then(() => {
                load_mailbox('inbox')
            }))
            document.querySelector('#emails-view').append(element);
        }

        const element1 = document.createElement('hr');
        document.querySelector('#emails-view').append(element1);

        const element2 = document.createElement('div');
        element2.innerHTML = email.body;
        document.querySelector('#emails-view').append(element2);
    });
}