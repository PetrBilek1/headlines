<template>
    <div :id="id" class="headline-change-wrapper mb-2" :key="change.id" :style="{ opacity: 0, borderColor: 'black', borderWidth: borderwidth + 'px', borderStyle: 'solid' }" ref="topdiv" v-on:mouseover="hovering = true" v-on:mouseleave="hovering = false" v-on:click="clicked = !clicked">
        <div class="row align-items-center">
            <div v-if="order" class="col-1 text-center">
                <b style="font-size: 22px;">#{{ order }}</b>
            </div>
            <div v-if="!order" :class="['col-12', 'col-lg-2', 'col-xl-1']">
                <b style="font-size: 16px;">{{ getLocalTimeString(change.detected) }}</b>
            </div>
            <div :class="[order ? 'col-lg-10' : 'col-lg-9 col-xl-10']">
                <div class="row" style="">
                    <div :class="['col-lg-6', 'd-none', hovering == true || clicked == true ? 'd-lg-none' : 'd-lg-block', 'single-line-text', 'col-anim', 'cursor-default']" style="color: #9E0937;" ref="titlebefore">
                        {{ change.titleBefore }}
                    </div>
                    <div :class="['col-lg-6', 'd-none', hovering == true || clicked == true ? 'd-lg-none' : 'd-lg-block', 'single-line-text', 'col-anim', 'cursor-default']" style="color: #148C55;" ref="titleafter">
                        {{ change.titleAfter }}
                    </div>
                    <div :class="['col-12', hovering == false && clicked == false ? 'd-lg-none' : 'd-lg-block', 'cursor-default']" v-html="getTitleDiffElement(change.titleBefore, change.titleAfter)">
                    </div>
                </div>
            </div>
            <div class="col-lg-1 text-center mt-2 mt-lg-0">
                <fai :icon="['fas', change.article.link.length > 0 ? 'link' : 'link-slash']" :class="['cursor-pointer']" v-on:click="redirect(change.article.link)"></fai>&nbsp;&nbsp;
                <span class="cursor-pointer" :style="[isUpvoted() == true ? 'color: #0A3640' : 'color: #888888', 'white-space: nowrap']" v-on:click.stop="upvote(change.id)">
                    <b><fai :icon="['fas', 'thumbs-up']"></fai>&nbsp;{{ change.upvoteCount }}</b>
                </span>
            </div>
        </div>
    </div>
</template>

<script>
import anime from 'animejs/lib/anime.es.js'
import moment from 'moment'
import * as differ from 'diff'
import axios from 'axios'
import endpoints from './../api/endpoints.js'

export default {
    name: 'HeadlineChangeRow',
    data() {
        return {
            alreadyAnimated: false,
            hovering: false,
            clicked: false
        }
    },
    props: {
        change: {},
        order: {
            default: null,
            type: Number
        },
        id: {
            type: String
        },
        backgroundcolor: {
            default: "#FFFFFF",
            type: String
        },
        borderwidth: {
            default: 0,
            type: Number
        },
        startAnimDelay: {
            default: 0,
            type: Number
        },
        mountedanimdelay: {
            default: 0,
            type: Number
        },
        animateonmount: {
            default: false,
            type: Boolean
        },
        usertoken: {
            default: null,
            type: String
        },
       userupvotes: {
            default() { return [] },
            type: Array
        }
    },
    methods: {
        mountedAnimation() {
            anime.timeline({ loop: false })
                .add({
                    targets: this.$refs["topdiv"],
                    opacity: [0, 1],
                    easing: "easeOutExpo",
                    duration: 1400,
                    delay: this.startAnimDelay + this.mountedanimdelay
                });
        },
        getLocalTimeString(dateTimeUTC) {
            const date = new Date(dateTimeUTC + '+00:00')

            return moment(date).format("HH:mm DD.MM.")
        },
        enterTitle(type) {
            var titleBefore = this.$refs["titlebefore"]
            var titleAfter = this.$refs["titleafter"]

            titleBefore.classList.remove("col-lg-6")
            titleAfter.classList.remove("col-lg-6")

            if (type === 'before') {
                titleBefore.classList.add("col-lg-10")
                titleAfter.classList.add("col-lg-2")
            }
            else if (type === 'after') {
                titleAfter.classList.add("col-lg-10")
                titleBefore.classList.add("col-lg-2")
            }
        },
        leaveTitle(type) {
            var titleBefore = this.$refs["titlebefore"]
            var titleAfter = this.$refs["titleafter"]

            titleBefore.classList.add("col-lg-6")
            titleAfter.classList.add("col-lg-6")

            if (type === 'before') {
                titleBefore.classList.remove("col-lg-10")
                titleAfter.classList.remove("col-lg-2")
            }
            else if (type === 'after') {
                titleAfter.classList.remove("col-lg-10")
                titleBefore.classList.remove("col-lg-2")
            }
        },
        getTitleDiffElement(before, after) {
            let span = null
            
            const diff = differ.diffWords(before, after),
                fragment = document.createDocumentFragment()

            diff.forEach((part) => {
                const color = part.added ? '#148C55' : part.removed ? '#9E0937' : '#000000'
                span = document.createElement('span')
                span.style.color = color
                span.appendChild(document.createTextNode(part.value))
                fragment.appendChild(span)
            });

            var div = document.createElement('div');
            div.appendChild(fragment.cloneNode(true));

            return div.innerHTML
        },
        upvote(id) {
            axios
                .post(endpoints.HeadlineChanges.Upvote(), {
                    headlineChangeId: id,
                    userToken: this.usertoken
                })
                .then(response => {
                    this.$emit('upvoted', response.data.upvotes)
                })
        },
        isUpvoted() {
            return this.userupvotes.some(x => x.type == 0 && x.targetId == this.change.id)
        },
        redirect(url) {
            if (url.length <= 0)
                return

            window.location.href = url
        }
    },
    emits: ['upvoted'],
    mounted() {
        if (this.animateonmount && !this.alreadyAnimated) {
            this.mountedAnimation()
            this.alreadyAnimated = true
        }
        else {
            this.$refs['topdiv'].style.opacity = 1
        }
    },
    updated() {
        this.$refs['topdiv'].style.opacity = 1
    }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
    .headline-change-wrapper {
        background-color: white;
        width: 100%;
        padding: 10px;
        border-radius: 15px;
    }

    .col-anim {
        transition: width 0.3s ease-in-out;
    }      

    .animate-max-height {
        transition: max-height 0.5s linear;
    }
</style>