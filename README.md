development-practice
====================

A basic project cycle, that simulates readers&amp;writers problem.

### What you must except from the project?
The project presents a basic software cycle, which includes;
* Requirements
* Design
* Coding
* Manual

### Ingredients
* RSD: Requirement Spesification Document. A document that explains customer needs.  Mostly prepared by an Analyst.
* DSD: Design Spesification Document. A document that holds architecture of code to be.  Mostly prepared by a Software Architect or Senior Developer.
* Source Code. You are familier with this I guess.
* SPM: Software Product Manual. A document that explain the usage of executable software. Mostly prepared by the one who codes.

### Summary
I guess most sw students, freelance developers or startups does not follow a proper development cycle. But If things get bigger, over time it will get harder to follow daily works, codes, and will be harder to develop as a team.
The project also have samples to show that how can you represent requirements and design objects with UML representation.

Do not fotget that, If you provide software projects to other companies, you must sign a contract. This project does not inlude a sample contract, but as a company, if you make some considirable money, I suggest you to get a professional support about contracts. After the agreement, present the RSD to other company and have an agreement on it, in order to prevent future confilicts.
This project is not best in class,also can be the simplest one,maybe wrong one, but migth help you set up the cycle that in your mind.


### About the code
The code may also help someone. It simulates [readers-writers problem](http://en.wikipedia.org/wiki/Readers-writers_problem). Writers are trying to write to a common buffer and readers are trying to read. Code might not be perfect but helps you to understand the issue.
Unlike the common one, threads do some random encryption work during their cycle. So the code have some samples about AES encryption. Readers removes the data after done, so you may think the code is not an exact readers-writers problem but also like [producer-consumer problem](http://en.wikipedia.org/wiki/Producer-consumer_problem).
