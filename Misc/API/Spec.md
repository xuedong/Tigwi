Tigwi - API Specification by L. de HARO, A. de MYTTENAERE and T. ZIMMERMANN 

#General presentation of the Tigwi API

##What you can do with our API

If you're a smartphone developper, you may be willing to make your own application to access Tigwi from everywhere. The simpliest way to do that is to use our API. Your software will be able to join our servers to get recent sent messages, see who's following who and even to post some content.

Our API provides a lot of functions. You can even create new lists, follow new people by adding them to one of your lists, follow public lists made by a complete stranger or tag messages you like the most.

If you're a webmaster, you would like to show your last posts ? You can use our API to make a rather simple javascript application doing that.

##Note about answers and errors

All answers are wrapped in the XML root `<Answer> </Answer>` whose sons are `<Content> </Content>` and `<Error\>`. Only one of them will appear. In the case where no error has occured but no content is expected in the answer, you will receive an empty _Error_ tag :

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Error />
    </Answer>

When an error occured you will get a description of it in the attribute _Code_ :

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Error Code="Subscription missing" />
    </Answer>

Finally, if you have a successful request expecting a real answer you could have something like :

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Content xsi:type="NewObject" Id="312e2061-3a79-4f82-a53b-e77af1ff0e59" />
    </Answer>

or

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Content xsi:type="Lists" Size="1">
      <List>
       <Id>312e2061-3a79-4f82-a53b-e77af1ff0e59</Id>
       <Name>Presidents of the US</Name>
      </List>
     </Content>
    </Answer>

Furthermore, for highly frequent errors, the answer HTTP status code will be changed from 200 (OK) to 400 (Bad Request), 403 (Forbidden), 404 (Not Found), 500 (Internal Server Error - not so likely to happen) or 501 (Not Implemented).

##Note about authentication

Some of our ressources require authentication. This is the case if you use any with a POST verb but also if you want to access to private data such as an user email, or an account private lists.

To authenticate, you need to send an HTTP cookie named _key_ whose value is a unique identifier, depending on the user and the application, that should have been generated before, along with the request.
This means that if you're writing requests manually you should write something like this :

    Cookie: key=312e2061-3a79-4f82-a53b-e77af1ff0e59

If authentication went wrong, you would have a code of error among :

* "No key cookie was sent"
* "Authentication failed" (if the key does not match any user)
* "User hasn't rights on this account"

You can handle your keys on the website (generating a new key and desactivating one to take the rights away from an application). There is also a method provided by the API to generate a new key (see below in the *User* section).

##Note about identifying an account

For every ressource depending on an account, you can give its name or its unique identifier.

For GET methods, the URL for the ressource depend on if you use the account name or the its unique identifier. To use the account name, you have the following URL :

http://api.tigwi.com/account/{action}/{accountName}/{number_if_needed}

http://api.tigwi.com/account/{action}/name={accountName}/{number_if_needed}

And to use the unique identifier :

http://api.tigwi.com/account/{action}/id={accountId}/{number_if_needed}

For POST methods, you have two tags, `<AccountName>` and `<AccountId>`, and you must choose one of them. Access through unique identifier is more direct. Thus, if you fill both `<AccountName>` and `<AccountId>` tags, only `<AccountId>` will be taken into consideration.


#Information about an *account*

##Read last messages wrote by someone

###Purpose

Get the number you want to of an account last sent messages.

###HTTP method

GET

###Request URL example

http://api.tigwi.com/account/messages/John_Smith/2

http://api.tigwi.com/account/messages/name=John_Smith/2

http://api.tigwi.com/account/messages/id=d818d509-e7eb-45b6-a56d-f472f075f433/2

###Response

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Content xsi:type="Messages" Size="2">
      <Message>
       <Id>eff8e9a3-ac58-4b38-96d1-04856a71aeb2</Id>
       <PostTime>2012-05-24T15:11:24.0396597</PostTime>
       <Poster>John_Smith</Poster>
       <Content>Tigwi is great ! This is my first message. I've just joined. I encourage you to do so.</Content>
      </Message>
      <Message>
       <Id>8b648f1c-0816-45be-9e60-30e4f2761801</Id>
       <PostTime>2012-05-24T15:11:38.7740347</PostTime>
       <Poster>John_Smith</Poster>
       <Content>Well, there doesn't seem to be a lot of people that heard of my last advice.</Content>
      </Message>
     </Content>
    </Answer>
  
Or, if an error occured, for example you thought the account name was Smith_John :

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Error Code="AccountNotFound" />
    </Answer>


###Information

* In **URL**, you should give the name or the unique identifier of the account whose messages you want to get.
* Then the number of messages. It is optional and default value is set to 20.
* In **Response**, _Size_ is the number of messages returned (different from the requested number if there are not enough messages to provide).


##Read again the recent messages you tagged

###Purpose

Get the number you want to of messages from your favourites. Authentication required.

###HTTP method

GET

###Request URL example

http://api.tigwi.com/account/taggedmessages/John_Smith/2

http://api.tigwi.com/account/taggedmessages/name=John_Smith/2

http://api.tigwi.com/account/taggedmessages/id=d818d509-e7eb-45b6-a56d-f472f075f433/2

###Response

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Content xsi:type="Messages" Size="2">
      <Message>
       <Id>c1a5ff05-7b50-4265-9a21-a0fee3bcfb77</Id>
       <PostTime>2012-05-29T10:46:57.4968697</PostTime>
       <Poster>John_Smith</Poster>
       <Content>I love Tigwi</Content>
      </Message>
      <Message>
       <Id>9e844bcd-e966-4d32-8c41-cf4a25c758a0</Id>
       <PostTime>2012-05-29T11:16:58.7093898</PostTime>
       <Poster>John_Smith</Poster>
       <Content>I love Tigwi

       retwiged by John_Smith</Content>
      </Message>
     </Content>
    </Answer>

###Information

* In **URL**, you should give the name or the unique identifier of the account whose favourites messages you want to get.
* Then the number of messages. It is optional and default value is set to 20.
* In **Response**, _Size_ is the number of messages returned (different from the requested number if there are not enough messages to provide).
* You **must** be authenticated as an authorized user of the account to see the tagged messages.


##See to which accounts someone subscribed

###Purpose

Get the number you want to of accounts in any of the lists followed by the given account. No special order provided.

If you're authenticated and you have the rights on the account, you will see private subscriptions (from private lists) along with public ones.

###HTTP method

GET

###Request URL example

http://api.tigwi.com/account/subscribedaccounts/John_Smith/2

http://api.tigwi.com/account/subscribedaccounts/name=John_Smith/2

http://api.tigwi.com/account/subscribedaccounts/id=d818d509-e7eb-45b6-a56d-f472f075f433/2

###Response

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Content xsi:type="Accounts" Size="2">
      <Account>
       <Id>ea4a4f69-fec3-4659-9235-0dcbcbd0d54f</Id>
       <Name>tttt</Name>
       <Description>I like turtles</Description>
      </Account>
      <Account>
       <Id>a3e56af5-9991-428e-8d8f-0b31f7e80e40</Id>
       <Name>Paul_Smith</Name>
       <Description>I'm the brother of @John_Smith</Description>
      </Account>
     </Content>
    </Answer>
 
###Information

* In **URL**, you should give the name or the unique identifier of the account whose subscriptions you want to get.
* Then the number of subscriptions. It is optional and default value is set to 20.
* In **Response**, _Size_ is the number of subscriptions returned (different from the requested number if there are not enough subscriptions to provide).
* If you're not authorized, you will only receive subscriptions from lists that the owner has set public.


##See which accounts are following someone

###Purpose

Get the number you want to of accounts that have subscribed a public list in which the given account appears. No special order provided

###HTTP method

GET

###Request URL example

http://api.tigwi.com/account/subscriberaccounts/John_Smith/1

http://api.tigwi.com/account/subscriberaccounts/name=John_Smith/1

http://api.tigwi.com/account/subscriberaccounts/id=d818d509-e7eb-45b6-a56d-f472f075f433/1

###Response

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Content xsi:type="Accounts" Size="1">
      <Account>
       <Id>a3e56af5-9991-428e-8d8f-0b31f7e80e40</Id>
       <Name>Paul_Smith</Name>
       <Description>I'm the brother of @John_Smith</Description>
      </Account>
     </Content>
    </Answer>

###Information
* In **URL**, you should give the name or the unique identifier of the account whose subscribers you want to see.
* Then the number of subscribers. It is optional and default value is set to 20.
* In **Response**, _Size_ is the number of subscribers returned (different from the requested number if there are not enough subscribers to provide).
* There is no way to get accounts following you through private lists.


##See the lists followed by someone

###Purpose

Get the number you want to of lists followed by the given account. No particular order provided.

If you're authenticated and you have the rights on the account, you will see private lists along with public ones.

###HTTP method

GET

###Request URL example

http://api.tigwi.com/account/subscribedlists/John_Smith/2

http://api.tigwi.com/account/subscribedlists/name=John_Smith/2

http://api.tigwi.com/account/subscribedlists/id=d818d509-e7eb-45b6-a56d-f472f075f433/2

###Response

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Content xsi:type="Lists" Size="1">
      <List>
       <Id>159e5dd6-4f65-43ac-aed9-f81348fc9b9d</Id>
       <Name>Work at Tigwi</Name>
       <Description>Void</Description>
      </List>
     </Content>
    </Answer>

###Information

* In **URL**, you should give the name or the unique identifier of the account whose subscriptions you want to get.
* Then the number of subscriptions. It is optional and default value is set to 20.
* In **Response**, _Size_ is the number of subscriptions returned (different from the requested number if there are not enough subscriptions to provide).
* If you're not authorized, you will only receive subscribed lists that the owner has set public.


##See in which lists someone appears

###Purpose

Get the number you want to of public lists where the given account appears. No particular order provided

###HTTP method

GET

###Request URL example

http://api.tigwi.com/account/subscriberlists/John_Smith/2

http://api.tigwi.com/account/subscriberlists/name=John_Smith/2

http://api.tigwi.com/account/subscriberlists/id=d818d509-e7eb-45b6-a56d-f472f075f433/2

###Response

??

###Information

* In **URL**, you should give the name or the unique identifier of the account whose subscribers you want to see.
* Then the number of subscribers. It is optional and default value is set to 20.
* In **Response**, _Size_ is the number of subscribers returned (different from the requested number if there are not enough subscribers to provide).
* There is no way to see private lists which subscribed to an account.


##See someone's owned lists

###Purpose

Get the number you want to of lists owned by the given account. No particular order provided.

If you're authenticated and you have the rights on the account, you will see private lists along with public ones.

###HTTP method

GET

###Request URL example

http://api.tigwi.com/account/ownedlists/John_Smith/2

http://api.tigwi.com/account/ownedlists/name=John_Smith/2

http://api.tigwi.com/account/ownedlists/id=d818d509-e7eb-45b6-a56d-f472f075f433/2

###Response

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Content xsi:type="Lists" Size="1">
      <List>
       <Id>159e5dd6-4f65-43ac-aed9-f81348fc9b9d</Id>
       <Name>Work at Tigwi</Name>
       <Description>Void</Description>
      </List>
     </Content>
    </Answer>

###Information

* In **URL**, you should give the name or the unique identifier of the account whose lists you want to see.
* Then the number of lists. It is optional and default value is set to 20.
* In **Response**, _Size_ is the number of lists returned (different from the requested number if there are not enough lists to provide).
* If you're not authorized, you will only receive lists that the owner has set public.


##Get main information about one account

###Purpose

Get main information about the given account.

###HTTP method

GET

###Request URL example

http://api.tigwi.com/account/maininfo/John_Smith

http://api.tigwi.com/account/maininfo/name=John_Smith

http://api.tigwi.com/account/maininfo/id=d818d509-e7eb-45b6-a56d-f472f075f433

###Response

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Content xsi:type="Account">
      <Id>d818d509-e7eb-45b6-a56d-f472f075f433</Id>
      <Name>John_Smith</Name>
      <Description>My account to say how much I love Tigwi</Description>
     </Content>
    </Answer>


#Actions on a *message*

**Remember :** since the following ressources use the POST verb, they require authentication.


##Write a message

###HTTP method

POST

###URL

http://api.tigwi.com/message/write

###Request examples

    <Write>
     <AccountName>John_Smith</AccountName>
     <Message>I love Tigwi</Message>
    </Write>

or

    <Write>
     <AccountId>d818d509-e7eb-45b6-a56d-f472f075f433</AccountId>
     <Message>I love Tigwi</Message>
    </Write>

###Response

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Content xsi:type="NewObject" Id="c1a5ff05-7b50-4265-9a21-a0fee3bcfb77" />
    </Answer>

###Information
* In **Request**, the size of your message is limited to 140 characters and this limit is verified by the server.
* In **Request**, `<AccountName>` is the name of the account where you intend to post a message.
* In **Request**, `<AccountId>` is the unique identifier of the account where you intend to post a message.
* In **Request**, if you use both `<AccountName>` and `<AccountId>`, only the `<AccountId>` will be used (particularly when they don't refer to the same account).
* You **must** be authenticated as an authorized user of the account to post a message.
* In **Response**, the unique identifier is the one of the new message.


##Copy a message

###Purpose

To copy a message is to write a message with the same content. You can copy messages sent by others.

###HTTP method

POST

###URL

http://api.tigwi.com/message/copy

###Request examples

    <Copy>
     <AccountName>John_Smith</AccountName>
     <MessageId>c1a5ff05-7b50-4265-9a21-a0fee3bcfb77</MessageId>
    </Copy>

or

    <Copy>
     <AccountId>d818d509-e7eb-45b6-a56d-f472f075f433</AccountId>
     <MessageId>c1a5ff05-7b50-4265-9a21-a0fee3bcfb77</MessageId>
    </Copy>

###Response

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Content xsi:type="NewObject" Id="9e844bcd-e966-4d32-8c41-cf4a25c758a0" />
    </Answer>

###Information
* In **Request**, `<AccountName>` is the name of the account where you want to copy a message.
* In **Request**, `<AccountId>` is the unique identifier of the account where you want to copy a message.
* In **Request**, if you use both `<AccountName>` and `<AccountId>`, only the `<AccountId>` will be used (in particular when they don't refer to the same account).
* In **Request**, `<MessageId>` is the unique identifier of the message you want to copy a message.
* You **must** be authenticated as an authorized user of the account to copy a message.


##Remove a message

Note : not implemented

###HTTP method

POST

###URL

http://api.tigwi.com/message/delete

###Request example

    <Delete>
     <MessageId>c1a5ff05-7b50-4265-9a21-a0fee3bcfb77</MessageId>
    </Delete>

###Response

If everything went well :

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Error />
    </Answer>

###Information

* You **must** be authenticated as an authorized user of the account which wrote the message.


##Add a message to an account's favourites

###Purpose

To tag a message as one of your favourites. Authentication required.

###HTTP method

POST

###URL

http://api.tigwi.com/message/tag

###Request examples

    <Tag>
     <AccountName>John_Smith</AccountName>
     <MessageId>c1a5ff05-7b50-4265-9a21-a0fee3bcfb77</MessageId>
    </Tag>

or

    <Tag>
     <AccountId>d818d509-e7eb-45b6-a56d-f472f075f433</AccountId>
     <MessageId>c1a5ff05-7b50-4265-9a21-a0fee3bcfb77</MessageId>
    </Tag>

###Response

If everything went well :

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Error />
    </Answer>

###Information

* In **Request**, `<AccountName>` is the name of the account where you intend to tag a message.
* In **Request**, `<AccountId>` is the unique identifier of the account where you intend to tag a message.
* In **Request**, if you use both `<AccountName>` and `<AccountId>`, only the `<AccountId>` will be used (in particular when they don't refer to the same account).
* You **must** be authenticated as an authorized user of the account where you intend to tag a message.


##Remove a message from an account's favourites

###Purpose

To remove a message from your favourites. Authentication required.

###HTTP method

POST

###URL

http://api.tigwi.com/message/untag

###Request

    <Untag>
     <AccountName>John_Smith</AccountName>
     <MessageId>c1a5ff05-7b50-4265-9a21-a0fee3bcfb77</MessageId>
    </Untag>

or

    <Untag>
     <AccountId>d818d509-e7eb-45b6-a56d-f472f075f433</AccountId>
     <MessageId>c1a5ff05-7b50-4265-9a21-a0fee3bcfb77</MessageId>
    </Untag>

###Response

If everything went well :

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Error />
    </Answer>

###Information

* In **Request**, `<AccountName>` is the name of the account where you intend to untag a message.
* In **Request**, `<AccountId>` is the unique identifier of the account where you intend to untag a message.
* In **Request**, if you use both `<AccountName>` and `<AccountId>`, only the `<AccountId>` will be used (in particular when they don't refer to the same account).
* You **must** be authenticated as an authorized user of the account where you intend to untag a message.


#Information about a *list*

##Get last messages sent to a list

###Purpose

Get the number you want to of last messages sent to the list given by its unique identifier.

###HTTP method

GET

###Request URL example

http://api.tigwi.com/list/messages/08a7f307-8ead-4b44-addc-ad9c482bdb26/1

###Response

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Content xsi:type="Messages" Size="1">
      <Message>
       <Id>8b85aa9c-1720-4cc7-ba41-c50d71841fbc</Id>
       <PostTime>2012-05-29T11:29:36.3591577</PostTime>
       <Poster>Global_daily</Poster>
       <Content>Welcome to our new followers !</Content>
      </Message>
     </Content>
    </Answer>

###Information

* In **URL**, you should give the unique identifier of the list whose messages you want to get.
* Then the number of messages. It is optional and default value is set to 20.
* In **Response**, _Size_ is the number of messages returned (different from the requested number if there are not enough messages to provide).


##Get accounts followed by the list

###Purpose

Get the number you want to of accounts followed by the given list.
No particular order provided

###HTTP method

GET

###Request URL example

http://api.tigwi.com/list/subscriptions/08a7f307-8ead-4b44-addc-ad9c482bdb26/1

###Response

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Content xsi:type="Accounts" Size="1">
      <Account>
       <Id>a3e56af5-9991-428e-8d8f-0b31f7e80e40</Id>
       <Name>Paul_Smith</Name>
       <Description>I'm the brother of @John_Smith</Description>
      </Account>
     </Content>
    </Answer>

###Information

* In **URL**, you should give the unique identifier of the list whose subscriptions you want to get.
* Then the number of subscriptions. It is optional and default value is set to 20.
* In **Response**, _Size_ is the number of subscriptions returned (different from the requested number if there are not enough subscriptions to provide).


##Get the list of accounts following a given list

###Purpose

Get the number you want to of accounts following the given list.
No particular order provided

###HTTP method

GET

###Request URL example

http://api.tigwi.com/list/subscribers/08a7f307-8ead-4b44-addc-ad9c482bdb26/1

###Response

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Content xsi:type="Accounts" Size="1">
      <Account>
       <Id>d818d509-e7eb-45b6-a56d-f472f075f433</Id>
       <Name>John_Smith</Name>
       <Description>My account to say how much I love Tigwi</Description>
      </Account>
     </Content>
    </Answer>

###Information

* In **URL**, you should give the unique identifier of the list whose subscribers you want to get.
* Then the number of subscribers. It is optional and default value is set to 20.
* In **Response**, _Size_ is the number of subscribers returned (different from the requested number if there are not enough subscribers to provide).


##Get a list's owner information

###Purpose

Get information about the owner of the given list.

###HTTP method

GET

###Request URL example

http://api.tigwi.com/list/owner/08a7f307-8ead-4b44-addc-ad9c482bdb26

###Response

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Content xsi:type="Account">
      <Id>d818d509-e7eb-45b6-a56d-f472f075f433</Id>
      <Name>John_Smith</Name>
      <Description>My account to say how much I love Tigwi</Description>
     </Content>
    </Answer>

###Information

* In **URL**, you should give the unique identifier of the list whose owner you want to get.


#Actions on a *list*

**Remember :** since the following ressources use the POST verb, they require authentication. You must be authenticated as an user authorized to use the given account.

##Create a list

###Purpose

If you wish to follow people, you must before create a new, empty list.
Authentication required.

###HTTP method

POST

###URL

http://api.tigwi.com/list/create/

###Request example

	<Create>
     <AccountName>John_Smith</AccountName>
     <ListInfo>
      <Name>Presidents of the US</Name>
      <Description>To keep touch</Description>
	  <IsPrivate>false</IsPrivate>
     </ListInfo>
    </Create>
or

	<Create>
     <AccountId>d818d509-e7eb-45b6-a56d-f472f075f433</AccountId>
     <ListInfo>
      <Name>Presidents of the US</Name>
      <Description>To keep touch</Description>
	  <IsPrivate>false</IsPrivate>
     </ListInfo>
    </Create>

###Response example

	<?xml version="1.0"?>
	<Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
	  <Content xsi:type="NewObject" Id="5af155a7-151e-4c6d-af3c-ec4af882aced" />
	</Answer>

###Information

* You **must** be authenticated and authorized to use the account.
* In **Request**, `<AccountName>` is the name of the account who wants to create the list _nameOfList_.
* In **Request**, `<AccountId>` is the unique identifier of the account who wants to create the list _nameOfList_.
* In **Request**, `<Name>` is the name you want to give to the new list.
* In **Request**, `<Description>` is a short text to remember what the list is about.
* In **Request**, `<IsPrivate>` value must be _false_ if you want the new list to be public or _true_ if only you should see that list.


##Make an account subscribe to a list

Authentication required.

###HTTP method

POST

###URL

http://api.tigwi.com/list/subscribe/

###Request
    
    <Subscribe>
     <List>08a7f307-8ead-4b44-addc-ad9c482bdb26</List>
	 <AccountName>John_Smith</AccountName>
    </Subscribe>

or

    <Subscribe>
     <List>08a7f307-8ead-4b44-addc-ad9c482bdb26</List>
     <AccountId>d818d509-e7eb-45b6-a56d-f472f075f433</AccountId>
    </Subscribe>

###Response

If everything went well :

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Error />
    </Answer>

###Information
* You **must** be authenticated as an authorized user of the given account to use this method.
* In **Request**, `<AccountName>` is the name of the account who wants to follow the list whose unique identifier is `<List>`.
* In **Request**, `<AccountId>` is the unique identifier of the account who wants to follow the list.
* It is possible to subscribe a private list just knowing its unique identifier, even if you're not the owner.


##Make an account unsubscribe from a list

Authentication required.

###HTTP method

POST

###URL

http://api.tigwi.com/list/unsubscribe/

###Request
    
    <Unsubscribe>
     <List>08a7f307-8ead-4b44-addc-ad9c482bdb26</List>
	 <AccountName>John_Smith</AccountName>
    </Unsubscribe>

or

    <Unsubscribe>
     <List>08a7f307-8ead-4b44-addc-ad9c482bdb26</List>
     <AccountId>d818d509-e7eb-45b6-a56d-f472f075f433</AccountId>
    </Unsubscribe>

###Response

If everything went well :

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Error />
    </Answer>

###Information
* You **must** be authenticated as an authorized user of the given account to use this method.
* In **Request**, `<AccountName>` is the name of the account who wants to stop following the list whose unique identifier is `<List>`.
* In **Request**, `<AccountId>` is the unique identifier of the account who wants to stop following the list.


#Modifying a *list*

**Remember :** since the following ressources use the POST verb, they require authentication. You must be authenticated as an user with appropriate autorization on the list you want to modify.


##Add an account to a list

###HTTP method

POST

###URL

http://api.tigwi.com/list/addaccount/

###Request example
    
    <AddAccount>
     <List>08a7f307-8ead-4b44-addc-ad9c482bdb26</List>
	 <AccountName>John_Smith</AccountName>
    </AddAccount>

or

    <AddAccount>
     <List>08a7f307-8ead-4b44-addc-ad9c482bdb26</List>
     <AccountId>d818d509-e7eb-45b6-a56d-f472f075f433</AccountId>
    </AddAccount>

###Response

If everything went well :

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Error />
    </Answer>
	
###Information

* You **must** be authenticated and authorized to use the owner of the list whose unique identifier is given by `<List>` to use this method.
* In **Request**, `<List>` is the unique identifier of the list who wants to follow the account `<Account>`.


##Remove an account from a list

###HTTP method

POST

###URL

http://api.tigwi.com/list/removeaccount/

###Request example

    <RemoveAccount>
     <List>08a7f307-8ead-4b44-addc-ad9c482bdb26</List>
	 <AccountName>John_Smith</AccountName>
    </RemoveAccount>

or

    <RemoveAccount>
     <List>08a7f307-8ead-4b44-addc-ad9c482bdb26</List>
     <AccountId>d818d509-e7eb-45b6-a56d-f472f075f433</AccountId>
    </RemoveAccount>

###Response

If everything went well :

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Error />
    </Answer>

###Information

* You **must** be authenticated and authorized to use the owner of the list whose unique identifier is given by `<List>` to use this method.
* In **Request**, `<List>` is the unique identifier of the list who wants to stop following the account `<Account>`.



#Get information about an *user*

###Purpose

Get main information about yourself as an user. Authentication required.

###HTTP method

GET

###URL

http://api.tigwi.com/user/maininfo/

###Response example

	<?xml version="1.0"?>
	<Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
	  <Content xsi:type="User">
	    <Login>John_Smith_login</Login>
	    <Email>falseaddress@yohoomail.com</Email>
	    <Id>c1881b11-aadb-4d3f-80c3-49d5b255ed7f</Id>
	  </Content>
	</Answer>

###Information

You don't need to give any detail on the user because you are identified through authentication.


#Handle your API authentication keys

##Generate a new API key

###Purpose

To allow a new application to access your data, you should first generate a new and unique API key.

###HTTP method

POST

###URL

http://api.tigwi.com/user/generatekey

###Request example

	<Identity>
	  <UserLogin>John_Smith_login</UserLogin>
	  <Password>not-the-real-password-don't-be-silly</Password>
	  <ApplicationName>myApp</ApplicationName>
	</Identity>

or

	<Identity>
	  <UserId>c1881b11-aadb-4d3f-80c3-49d5b255ed7f</UserId>
	  <Password>not-the-real-password-don't-be-silly</Password>
	  <ApplicationName>myApp</ApplicationName>
	</Identity>

###Response

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Content xsi:type="NewObject" Id="312e2061-3a79-4f82-a53b-e77af1ff0e59" />
    </Answer>

But if the login or the password were wrong, you will receive :

    <?xml version="1.0"?>
    <Answer xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
     <Error Code="Authentication failed" />
    </Answer>


###Information

* This is the only ressource that asks for your login and password.
* A cookie will be set in the response headers beside giving the key into the response body.
* You shouldn't give the same key to two different applications.
* All keys can be desactivated on the website.