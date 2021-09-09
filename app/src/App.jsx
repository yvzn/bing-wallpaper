/*
   Copyright 2021 Yvan Razafindramanana

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

import React from "react";
import Skeleton from "react-loading-skeleton";

import getWallpapers from "./service";

class Wallpapers extends React.Component {
	constructor() {
		const self = super();
		self.state = { images: new Array(4).fill({ status: "loading" }) };
	}

	componentDidMount() {
		var self = this;
		getWallpapers().then((newImages) => {
			self.setState((previousState) => {
				const images = newImages.map((image) => ({
					...image,
					status: "loaded",
				}));
				return {
					...previousState,
					images,
				};
			});
		});
	}

	render() {
		return (
			<section>
				{this.state.images.map((image, index) => (
					<WallpaperCard image={image} key={index} />
				))}
			</section>
		);
	}
}

function WallpaperCard({ image }) {
	return (
		<article>
			<ImageTitle image={image} />
			<ImagePreview image={image} />
			<ImageCopyright image={image} />
		</article>
	);
}

function ImagePreview({ image }) {
	return (
		<p>
			{image.status === "loading" && <Skeleton />}
			{image.status === "loaded" && (
				<a href={image.large.url}>
					<img
						src={image.thumbnail.url}
						alt={image.title}
						loading="lazy"
					/>
				</a>
			)}
		</p>
	);
}

function ImageTitle({ image }) {
	return (
		<h2>
			{image.status === "loading" && <Skeleton />}
			{image.status === "loaded" && (
				<a href={image.large.url}>{image.title}</a>
			)}
		</h2>
	);
}

function ImageCopyright({ image }) {
	return (
		<footer>
			{image.status === "loading" && <Skeleton />}
			{image.status === "loaded" && image.copyright}
		</footer>
	);
}

function App() {
	return <Wallpapers />;
}

export default App;
