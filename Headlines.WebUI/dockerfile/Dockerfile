# base image
FROM node:18-alpine
RUN addgroup --system nonroot 
RUN adduser --system nonroot -G nonroot

# set working directory
WORKDIR /app

# add `/app/node_modules/.bin` to $PATH
ENV PATH /app/node_modules/.bin:$PATH

# install and cache app dependencies
COPY Headlines.WebUI/package.json /app/package.json
RUN npm install

# start app
USER nonroot
CMD ["npm", "run", "serve"]