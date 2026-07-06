# Chatbot
This is my intuitive approach to making a chatbot. This is a relatively simple console application that does not use any LLMs to generate responses. As you can see from the conversation below, responses can make sense, but are also seemingly random and unrelated to user input sometimes:

<img width="569" height="411" alt="image" src="https://github.com/user-attachments/assets/6ace4092-f85e-4830-8ef9-f1490c7caeaf" />


# The Dataset

The key part of this program is the dataset which is a text file containing many conversation pairs. Each pair contains a message and a response to that message. The dataset is a collection of data that I have collected online from various sources which includes public chat logs and movie dialogue.

These pairs are stored in the text file in the format:

message|||response


# Choosing a Response

When the user enters a message, the program finds the closest string in the dataset to the user input string. A combination of string similarity methods are used to find the best match, including Levenshtein Distance, Jaccard Similarity, and Cosine Similarity. Then the program simply returns the corresponding response to the match in the dataset from the message-response pair. This is the chatbot's response to user input.

# Learning

The program does learn from user input in a way, as the user's follow-up responses to the chatbot's responses are automatically added as more message-response pairs to the dataset in the format: chatbotResponse|||userResponse

# Limitations

This basic approach to a conversational chatbot does have its limitations:
- The chatbot has no memory of previous user inputs or responses in the conversation, leading to lack of continuity in conversation
- Responses can often be nonsensical if there is not already a good match in the dataset
- Due to the dataset size and format, it is slow and inefficient to query. This results in the chatbot taking a few seconds to respond to user input. This delay will only get worse as the dataset size increases.

