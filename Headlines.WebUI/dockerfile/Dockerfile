# base image
FROM node:18-alpine

# set working directory
WORKDIR /app

# add `/app/node_modules/.bin` to $PATH
ENV PATH /app/node_modules/.bin:$PATH

# install and cache app dependencies
COPY Headlines.WebUI/package.json /app/package.json
RUN npm install
#RUN npm install vue-router@4
#RUN npm install bootstrap@5.2.3 jquery @popperjs/core
#RUN npm install animejs
#RUN npm install moment
#RUN npm install diff
#RUN npm install axios
#RUN npm install vue-cookies
#RUN npm install vuex@next
#RUN npm install @fortawesome/fontawesome-svg-core
#RUN npm install @fortawesome/free-solid-svg-icons
#RUN npm install @fortawesome/vue-fontawesome@latest-3

# start app
CMD ["npm", "run", "serve"]